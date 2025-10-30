using UnityEngine;
using System.Collections;

public class TimeManager : Singleton<TimeManager>
{
    [SerializeField] private float currentTime = 0f;     // 게임 시간
    public float GameMaxTime = 375f;    // 게임 최대 시간 (test용 100초, 실제 375초)
    private bool isTimerRunning = false; // 게임 타이머 실행 여부
    private Coroutine timerCoroutine;    // 게임 타이머 코루틴

    [Header("Events")]
    public System.Action<float> OnTimeUpdated;
    public System.Action<bool> OnDayNightChanged;
    public System.Action<int> OnPhaseChanged;

    public float CurrentTime => currentTime;
    public int CurrentPhase =>  Mathf.Clamp(Mathf.FloorToInt(currentTime / (GameMaxTime / 5)) + 1, 1, 5);
    public bool IsLastPhaseOver => currentTime >= GameMaxTime;
    
    public bool IsDayTime
    {
        get
        {
            if (currentTime >= GameMaxTime)
                return false;
            else
            {
                float cycleTime = currentTime % (GameMaxTime / 5);
                if (cycleTime < (GameMaxTime / 12.5f))  // 12.5f = 30초
                    return true;
                else
                    return false;
            }
        }
    }
    public bool IsTimerRunning => isTimerRunning;

    public void StartGameTimer()    // 게임 타이머 시작
    {
        if (!isTimerRunning)
        {
            currentTime = 0f;
            isTimerRunning = true;
            timerCoroutine = StartCoroutine(TimerCoroutine());
        }
    }

    public void StopGameTimer()    // 게임 타이머 정지
    {
        isTimerRunning = false;
        if (timerCoroutine != null)
        {
            StopCoroutine(timerCoroutine);
        }
    }

    public void PauseGameTimer()    // 게임 타이머 일시정지
    {
        if (isTimerRunning)
        {
            StopCoroutine(timerCoroutine);
        }
    }

    public void ResumeGameTimer()   // 게임 타이머 재개
    {
        if (isTimerRunning && timerCoroutine == null)
        {
            timerCoroutine = StartCoroutine(TimerCoroutine());
        }
    }

    public void ResetTimer()
    {
        currentTime = 0f;
        if (isTimerRunning)
        {
            StopGameTimer();
        }
    }

    private IEnumerator TimerCoroutine()
    {
        bool previousDayNight = IsDayTime;
        int previousPhase = CurrentPhase;

        while (isTimerRunning)
        {
            yield return new WaitForSeconds(0.1f);
            currentTime += 0.1f;

            // 시간 업데이트 이벤트 실행
            OnTimeUpdated?.Invoke(currentTime);

            // 낮/밤 상태 체크
            bool currentDayNight = IsDayTime;
            if (currentDayNight != previousDayNight)
            {
                OnDayNightChanged?.Invoke(currentDayNight);
                UIManager.Instance.OnNoticeAdded?.Invoke(
                    currentDayNight ? "낮이 되었습니다." : "밤이 되었습니다.",
                    NoticeType.System
                );
                previousDayNight = currentDayNight;
            }

            // 페이즈 체크 (2분 30초 = 150초마다)
            int currentPhase = CurrentPhase;
            if (currentPhase <= 5 && currentPhase != previousPhase)
            {
                // 페이즈 변경 이벤트 실행
                OnPhaseChanged?.Invoke(currentPhase);
                previousPhase = currentPhase;
            }
        }
    }
}
