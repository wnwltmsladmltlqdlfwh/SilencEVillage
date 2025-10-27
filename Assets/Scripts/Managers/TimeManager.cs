using UnityEngine;
using System.Collections;

public class TimeManager : Singleton<TimeManager>
{
    [SerializeField] private float currentTime = 0f;     // 게임 시간
    private bool isTimerRunning = false; // 게임 타이머 실행 여부
    private Coroutine timerCoroutine;    // 게임 타이머 코루틴

    [Header("Events")]
    public System.Action<float> OnTimeUpdated;
    public System.Action<bool> OnDayNightChanged;
    public System.Action<int> OnPhaseChanged;

    public float CurrentTime => currentTime;
    public int CurrentPhase =>  Mathf.Clamp(Mathf.FloorToInt(currentTime / 150f) + 1, 1, 5);
    
    public bool IsDayTime
    {
        get
        {
            if (currentTime >= 750f)
                return false;
            else
            {
                float cycleTime = currentTime % 150f;
                if (cycleTime < 60f)
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
            Debug.Log("게임 타이머 시작");
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
                previousDayNight = currentDayNight;
            }

            // 페이즈 체크 (2분 30초 = 150초마다)
            int phase = Mathf.FloorToInt(currentTime / 150f) + 1;
            if (phase <= 5)
            {
                // 페이즈 변경 이벤트 실행
                OnPhaseChanged?.Invoke(phase);
            }
        }
    }
}
