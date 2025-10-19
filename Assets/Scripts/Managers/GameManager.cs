using UnityEngine;
using System.Collections;

public class GameManager : Singleton<GameManager>
{
    [Header("Game Settings")]
    [SerializeField] private float gameTimeLimit = 750f; // 12분 30초 (750초)
    
    [Header("Game State")]
    public GameState currentGameState = GameState.None;
    private bool isGameActive = false;
    private int currentPhase = 1;
    
    [Header("Events")]
    public System.Action<GameState> OnGameStateChanged;
    public System.Action<AreaType> OnAreaChanged;
    public System.Action<int> OnPhaseChanged;
    public System.Action OnGameVictory;
    public System.Action OnGameDefeat;
    
    private void Start()
    {
        InitializeGame();
        SubscribeToTimeManager();
    }
    
    private void OnDestroy()
    {
        UnsubscribeFromTimeManager();
    }
    
    private void InitializeGame()
    {
        currentGameState = GameState.None;
        currentPhase = 1;
        isGameActive = false;
    }
    
    private void SubscribeToTimeManager()
    {
        // TimeManager 이벤트 구독
        TimeManager.Instance.OnTimeUpdated += OnTimeUpdated;
        TimeManager.Instance.OnPhaseChanged += OnPhaseChangedFromTimeManager;
    }
    
    private void UnsubscribeFromTimeManager()
    {
        // 이벤트 구독 해제
        if (TimeManager.Instance != null)
        {
            TimeManager.Instance.OnTimeUpdated -= OnTimeUpdated;
            TimeManager.Instance.OnPhaseChanged -= OnPhaseChangedFromTimeManager;
        }
    }
    
    // 1. LobbyScene에서 GameScene으로 전환 시 호출
    public void StartGame()
    {
        Debug.Log("게임 시작 준비");
        ShowTutorial();
    }
    
    // 2. 게임 방법 팝업 표시
    private void ShowTutorial()
    {
        currentGameState = GameState.Tutorial;
        OnGameStateChanged?.Invoke(currentGameState);
        
        // UIManager를 통해 튜토리얼 팝업 표시
        UIManager.Instance.ShowTutorialPopup();
    }
    
    // 3. 팝업 종료 시 게임 시작
    public void StartGameplay()
    {
        currentGameState = GameState.Playing;
        isGameActive = true;
        
        OnGameStateChanged?.Invoke(currentGameState);
        
        // TimeManager 시작
        TimeManager.Instance.StartGameTimer();
        
        Debug.Log("게임 플레이 시작!");
    }
    
    // TimeManager에서 시간 업데이트 알림을 받음
    private void OnTimeUpdated(float currentTime)
    {
        // 게임 종료 조건 체크 (12분 30초 경과)
        if (currentTime >= gameTimeLimit)
        {
            CheckVictoryCondition();
        }
    }
    
    // TimeManager에서 페이즈 변경 알림을 받음
    private void OnPhaseChangedFromTimeManager(int phase)
    {
        currentPhase = phase;
        OnPhaseChanged?.Invoke(phase);
        Debug.Log($"Phase {phase} 시작!");
    }
    
    // 4-1. 승리 조건 확인 (마을 입구 도달)
    public void CheckVictoryCondition()
    {
        // AreaType.Entrance에 도달했는지 확인하는 로직
        if (IsPlayerAtEntrance())
        {
            GameVictory();
        }
    }
    
    private bool IsPlayerAtEntrance()
    {
        // 플레이어가 마을 입구에 있는지 확인하는 로직
        // AreaManager나 PlayerManager에서 확인
        // 임시로 true 반환 (실제 구현 시 수정 필요)
        return true;
    }
    
    public void GameVictory()
    {
        currentGameState = GameState.Victory;
        isGameActive = false;
        
        TimeManager.Instance.StopGameTimer();
        OnGameVictory?.Invoke();
        OnGameStateChanged?.Invoke(currentGameState);
        
        Debug.Log("게임 승리!");
        
        // 승리 UI 표시 후 로비로 이동
        StartCoroutine(ShowVictoryAndReturnToLobby());
    }
    
    public void GameDefeat()
    {
        currentGameState = GameState.Defeat;
        isGameActive = false;
        
        TimeManager.Instance.StopGameTimer();
        OnGameDefeat?.Invoke();
        OnGameStateChanged?.Invoke(currentGameState);
        
        Debug.Log("게임 패배!");
        
        // 4-2. 사망 원인 팝업 표시
        UIManager.Instance.ShowDeathCausePopup();
    }
    
    // 5. 패배 팝업 종료 시 로비로 이동
    public void ReturnToLobby()
    {
        currentGameState = GameState.GameOver;
        OnGameStateChanged?.Invoke(currentGameState);
        
        // 로비 씬으로 이동
        SceneManagerEX.Instance.LoadScene("LobbyScene");
    }
    
    private IEnumerator ShowVictoryAndReturnToLobby()
    {
        // 승리 UI 표시
        UIManager.Instance.ShowVictoryPopup();
        
        // 잠시 대기
        yield return new WaitForSeconds(3f);
        
        // 로비로 이동
        ReturnToLobby();
    }
    
    // 게임 일시정지/재개
    public void PauseGame()
    {
        if (isGameActive)
        {
            currentGameState = GameState.Pause;
            Time.timeScale = 0f;
            TimeManager.Instance.PauseGameTimer();
            OnGameStateChanged?.Invoke(currentGameState);
        }
    }
    
    public void ResumeGame()
    {
        if (isGameActive)
        {
            currentGameState = GameState.Playing;
            Time.timeScale = 1f;
            TimeManager.Instance.ResumeGameTimer();
            OnGameStateChanged?.Invoke(currentGameState);
        }
    }
    
    // 게임 리셋
    public void ResetGame()
    {
        InitializeGame();
        TimeManager.Instance.ResetTimer();
    }
    
    // 현재 게임 상태 확인
    public bool IsGamePlaying()
    {
        return currentGameState == GameState.Playing;
    }
    
    public int GetCurrentPhase()
    {
        return currentPhase;
    }
}
