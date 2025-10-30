using UnityEngine;
using System.Collections;

public class GameManager : Singleton<GameManager>
{
    [Header("Game State")]
    public GameState currentGameState = GameState.None;
    private bool isGameActive = false;
    private bool isPlayerProtected = false;
    [SerializeField] private float playerProtectionDuration = 10f;
    public bool IsPlayerProtected => isPlayerProtected;
    
    [Header("Events")]
    public System.Action<GameState> OnGameStateChanged;
    public System.Action<AreaType> OnAreaChanged;
    public System.Action<int> OnPlayerProtectionChanged;
    public System.Action OnGameVictory;
    public System.Action<EnemyDataSO> OnGameDefeat;
    
    private void Start()
    {
        InitializeGameScene();
        InitForStartGame();
    }
    
    private void InitializeGameScene()
    {
        currentGameState = GameState.None;
        isGameActive = false;
        isPlayerProtected = false;
    }
    
    // 1. LobbyScene에서 GameScene으로 전환 시 호출
    public void InitForStartGame()
    {
        Debug.Log("게임 시작 준비");
        // 매니저 인스턴스 체크
        if(AreaManager.Instance == null) return;
        if(EnemyManager.Instance == null) return;
        if(UIManager.Instance == null) return;
        
        // 매니저 초기화
        AreaManager.Instance.InitAreaManager();
        EnemyManager.Instance.InitEnemyManager();
        UIManager.Instance.InitUIManager();

        // 튜토리얼 표시
        if(!OptionManager.Instance.IsTutorialShown)
        {
            ShowTutorial();
        }
        else
        {
            StartGameplay();
        }
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
    
    public void GameVictory()
    {
        currentGameState = GameState.Victory;
        isGameActive = false;
        
        TimeManager.Instance.StopGameTimer();
        OnGameVictory?.Invoke();
        OnGameStateChanged?.Invoke(currentGameState);
        
        Debug.Log("게임 승리!");
    }
    
    public void GameDefeat(EnemyDataSO enemyData)
    {
        currentGameState = GameState.Defeat;
        isGameActive = false;
        
        TimeManager.Instance.StopGameTimer();
        OnGameStateChanged?.Invoke(currentGameState);
        OnGameDefeat?.Invoke(enemyData);
        
        Debug.Log("게임 패배!");
    }
    
    // 5. 패배 팝업 종료 시 로비로 이동
    public void ReturnToLobby()
    {
        currentGameState = GameState.GameOver;
        OnGameStateChanged?.Invoke(currentGameState);
        
        // 로비 씬으로 이동
        SceneManagerEX.Instance.LoadScene(SceneType.LobbyScene.ToString());
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
        InitializeGameScene();
        TimeManager.Instance.ResetTimer();
    }

    // 플레이어 보호 상태 설정
    public void StartPlayerProtection()
    {
        if(isPlayerProtected) return;

        isPlayerProtected = true;
        _= StartCoroutine(ProtectionCoroutine());
    }

    // 플레이어 보호 코루틴
    private IEnumerator ProtectionCoroutine()
    {
        float elapsedTime = 0f;
        while (elapsedTime < playerProtectionDuration)
        {
            elapsedTime += Time.deltaTime;
            OnPlayerProtectionChanged?.Invoke((int)(playerProtectionDuration - elapsedTime));
            yield return null;
        }
        isPlayerProtected = false;
    }
    
    // 현재 게임 상태 확인
    public bool IsGamePlaying()
    {
        return currentGameState == GameState.Playing;
    }
}
