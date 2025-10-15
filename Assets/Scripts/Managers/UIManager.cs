using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using TMPro;
using System.Threading.Tasks;

public class UIManager : Singleton<UIManager>
{

    [SerializeField] private Image fadeImage;

    [Header("Popups")]
    [SerializeField] private GameObject tutorialPopup;
    [SerializeField] private GameObject victoryPopup;
    [SerializeField] private GameObject defeatPopup;

    [Header("Texts")]
    [SerializeField] private TMP_Text timeText;
    [SerializeField] private TMP_Text dayText;
    [SerializeField] private TMP_Text currentAreaText;

    [SerializeField] private Image currentAreaImage;
    private bool isMoving = false;

    private Dictionary<UIType, GameObject> uiList = new Dictionary<UIType, GameObject>();

    public void OpenUI(UIType uiType)
    {
        uiList[uiType].SetActive(true);
    }

    private async Task InitAreaImageList()
    {
        Debug.Log("UIManager: Area 이미지 로딩 요청...");

        // ResourceManager를 통해 이미지 로딩
        await ResourceManager.Instance.LoadAreaImagesAsync();

        Debug.Log("UIManager: Area 이미지 로딩 완료");
    }


    public void ShowTutorialPopup()
    {
        tutorialPopup.SetActive(true);
    }
    public void HideTutorialPopup()
    {
        if (tutorialPopup != null)
            tutorialPopup.SetActive(false);
    }

    public void ShowVictoryPopup()
    {
        if (victoryPopup != null)
            victoryPopup.SetActive(true);
    }

    public void ShowDeathCausePopup()
    {
        if (defeatPopup != null)
            defeatPopup.SetActive(true);
    }

    public void UpdateTimerDisplay(float currentTime, float maxTime)
    {
        if (timeText != null)
        {
            int minutes = Mathf.FloorToInt(currentTime / 60);
            int seconds = Mathf.FloorToInt(currentTime % 60);
            timeText.text = $"{minutes:00}:{seconds:00}";
        }
    }

    public void UpdatePhaseDisplay(int phase)
    {
        if (dayText != null)
        {
            dayText.text = $"{phase}";
        }
    }

    private async void Start()
    {
        await InitAreaImageList();
        SubscribeToTimeManager();
    }

    private void OnDestroy()
    {
        UnsubscribeFromTimeManager();
        // ResourceManager가 메모리 관리를 담당하므로 여기서는 구독 해제만
    }

    private void SubscribeToTimeManager()
    {
        // TimeManager 이벤트 구독
        if (TimeManager.Instance != null)
        {
            TimeManager.Instance.OnTimeUpdated += OnTimeUpdated;
            TimeManager.Instance.OnPhaseChanged += OnPhaseChanged;
            TimeManager.Instance.OnDayNightChanged += (bool isDayLight) => SetCurrentAreaImage(isDayLight);
            GameManager.Instance.OnAreaChanged += (Area area) => SetCurrentAreaImage(area);
        }
    }

    private void UnsubscribeFromTimeManager()
    {
        // 이벤트 구독 해제
        if (TimeManager.Instance != null)
        {
            TimeManager.Instance.OnTimeUpdated -= OnTimeUpdated;
            TimeManager.Instance.OnPhaseChanged -= OnPhaseChanged;
            TimeManager.Instance.OnDayNightChanged -= (bool isDayLight) => SetCurrentAreaImage(isDayLight);
            GameManager.Instance.OnAreaChanged -= (Area area) => SetCurrentAreaImage(area);
        }
    }

    // TimeManager에서 시간 업데이트 알림을 받음
    private void OnTimeUpdated(float currentTime)
    {
        UpdateTimerDisplay(currentTime, 750f); // 12분 30초 = 750초
    }

    // TimeManager에서 페이즈 변경 알림을 받음
    private void OnPhaseChanged(int phase)
    {
        UpdatePhaseDisplay(phase);
    }

    // 팝업 버튼 이벤트들
    public void OnTutorialPopupClose()
    {
        HideTutorialPopup();
        // GameManager에 게임 시작 알림
        GameManager.Instance.StartGameplay();
    }

    public void OnDefeatPopupClose()
    {
        if (defeatPopup != null)
            defeatPopup.SetActive(false);
        // GameManager에 로비로 이동 요청
        GameManager.Instance.ReturnToLobby();
    }

    public void SetCurrentAreaImage(bool isDayLight)
    {
        Area currentArea = GameManager.Instance.currentAreaType;
        SetAreaImageByType(currentArea, isDayLight);
    }

    public void SetCurrentAreaImage(Area area)
    {
        bool isDayLight = TimeManager.Instance.IsDayLight;
        SetAreaImageByType(area, isDayLight);
    }

    private void SetAreaImageByType(Area area, bool isDayLight)
    {
        // ResourceManager에서 이미지 가져오기
        Sprite targetSprite = ResourceManager.Instance.GetAreaImage(area, isDayLight);

        // 실제 UI Image에 스프라이트 설정
        if (targetSprite != null && currentAreaImage != null)
        {
            currentAreaImage.sprite = targetSprite;
            Debug.Log($"UIManager: 이미지 변경 - {area} - {(isDayLight ? "낮" : "밤")}");
        }
        else
        {
            Debug.LogWarning($"UIManager: 이미지를 찾을 수 없습니다 - {area} - {(isDayLight ? "낮" : "밤")}");
        }
    }

    #region Player Movement UI
    public void PlayerMovement()
    {
        if(isMoving)    return;
        
        StartCoroutine(AreaUIMovementCoroutine());
    }

    // 현재 지역 UI를 위 아래로 이동하는 코루틴 (이동 애니메이션)
    public IEnumerator AreaUIMovementCoroutine()
    {
        isMoving = true;
        RectTransform areaRect = currentAreaImage.rectTransform;
        Vector2 originalPosition = areaRect.anchoredPosition;

        float moveDistance = 10f;
        float duration = 0.4f;

        // 위 아래로 이동 후 원래 위치로 돌아옴
        yield return SmoothMoveAnchored(areaRect, new Vector2(originalPosition.x, originalPosition.y + moveDistance), duration);
        yield return SmoothMoveAnchored(areaRect, new Vector2(originalPosition.x, originalPosition.y - moveDistance), duration);
        yield return SmoothMoveAnchored(areaRect, originalPosition, duration);
        isMoving = false;
    }

    // 앵커 위치를 부드럽게 이동하는 코루틴
    private IEnumerator SmoothMoveAnchored(RectTransform target, Vector2 targetPosition, float duration)
    {
        Vector2 startPosition = target.anchoredPosition;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            // 부드러운 애니메이션
            t = Mathf.SmoothStep(0f, 1f, t);

            target.anchoredPosition = Vector2.Lerp(startPosition, targetPosition, t);
            yield return null;
        }

        target.anchoredPosition = targetPosition;
    }

    public IEnumerator FadeOutUI(float fadeSpeed)
    {
        float alpha = 0f;
        while (alpha < 1f)
        {
            alpha += Time.deltaTime * fadeSpeed;
            fadeImage.color = new Color(0, 0, 0, alpha);
            yield return null;
        }
        fadeImage.color = new Color(0, 0, 0, 1f);
        fadeImage.raycastTarget = true;
    }

    public IEnumerator FadeInUI(float fadeSpeed)
    {
        float alpha = 1f;
        while (alpha > 0f)
        {
            alpha -= Time.deltaTime * fadeSpeed;
            fadeImage.color = new Color(0, 0, 0, alpha);
            yield return null;
        }
        fadeImage.color = new Color(0, 0, 0, 0f);
        fadeImage.raycastTarget = false;
    }

    #endregion
}
