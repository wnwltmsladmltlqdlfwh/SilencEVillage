using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using TMPro;
using System.Threading.Tasks;
using System;

public class UIManager : Singleton<UIManager>
{
    [Header("Popups")]
    [SerializeField] private GameObject tutorialPopup;
    [SerializeField] private GameObject victoryPopup;
    [SerializeField] private GameObject defeatPopup;
    [SerializeField] private ItemOptionPanel craftingPopup;

    [Header("Texts")]
    [SerializeField] private TMP_Text timeText;
    [SerializeField] private TMP_Text dayText;
    [SerializeField] private TMP_Text currentAreaText;
    [SerializeField] private TMP_Text playerProtectionText;

    [Header("Images")]
    [SerializeField] private Image movementAreaImage;
    [SerializeField] private Image backgroundAreaImage;
    [SerializeField] private Image movementIcon;
    private bool isMoving = false;  // 플레이어가 탐사 or 이동 중
    public bool IsMoving => isMoving;
    [SerializeField] private Image progressbarImage;
    private bool isProgress = false;    // 플레이어가 제작 or 사용 중
    public bool IsProgress => isProgress;
    private bool isImageLoadComplete = false;   // 이미지 로딩 완료 여부

    public Action<string, NoticeType> OnNoticeAdded; // 알림 추가 이벤트

    public void InitUIManager()
    {
        if (AreaManager.Instance == null)
        {
            Debug.LogError("UIManager: AreaManager 인스턴스가 없습니다!");
            return;
        }

        AreaBase currentArea = AreaManager.Instance.PlayerCurrentArea;
        if (currentArea == null)
        {
            Debug.LogError("UIManager: 현재 지역 정보가 없습니다!");
            return;
        }

        AreaType currentAreaType = currentArea.AreaType;
        bool isDayLight = TimeManager.Instance.IsDayTime;

        if (AreaManager.AreaNameDictionary.ContainsKey(currentAreaType))
        {
            currentAreaText.text = AreaManager.AreaNameDictionary[currentAreaType];
        }
        else
        {
            Debug.LogError("UIManager: 현재 지역 이름이 없습니다!");
            return;
        }

        playerProtectionText.text = " ";

        StartImageLoadingAsync(currentAreaType, isDayLight);

        SubscribeEvents();

        Debug.Log("UIManager: 초기화 완료");
    }

    //  비동기 이미지 로딩 (별도 실행)
    private async void StartImageLoadingAsync(AreaType initialArea, bool isDayLight)
    {
        Debug.Log("UIManager: 이미지 로딩 시작 (백그라운드)...");

        try
        {
            // ResourceManager를 통해 이미지 로딩
            await ResourceManager.Instance.LoadAreaImagesAsync();

            isImageLoadComplete = true;
            Debug.Log("UIManager: 이미지 로딩 완료");

            // 로딩 완료 후 이미지 적용
            ApplyAreaImage(initialArea, isDayLight);
        }
        catch (Exception e)
        {
            Debug.LogError($"UIManager: 이미지 로딩 실패 - {e.Message}");
        }
    }


    // private async Task InitAreaImageList()
    // {
    //     Debug.Log("UIManager: Area 이미지 로딩 요청...");

    //     // ResourceManager를 통해 이미지 로딩
    //     await ResourceManager.Instance.LoadAreaImagesAsync();

    //     Debug.Log("UIManager: Area 이미지 로딩 완료");
    // }

    #region Popup UI
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

    public void CloseVictoryPopup()
    {
        if (victoryPopup != null)
            victoryPopup.SetActive(false);
        // GameManager에 로비로 이동 요청
        GameManager.Instance.ReturnToLobby();
    }

    public void ShowDeathCausePopup(EnemyDataSO enemyData)
    {
        if (defeatPopup != null)
            defeatPopup.SetActive(true);
        defeatPopup.GetComponent<DefeatPopup>().SetupInfoTheEnemy(enemyData);
    }

    public void CloseDefeatPopup()
    {
        if (defeatPopup != null)
            defeatPopup.SetActive(false);
        // GameManager에 로비로 이동 요청
        GameManager.Instance.ReturnToLobby();
    }

    public void ShowCraftingPopup()
    {
        if (craftingPopup.gameObject.activeSelf) return;

        craftingPopup.gameObject.SetActive(true);
        craftingPopup.InitializeCraftingPopup(ItemManager.Instance.SelectedInventoryIndex);
    }

    public void CloseCraftingPopup()
    {
        craftingPopup.gameObject.SetActive(false);
    }
    #endregion


    # region Time Updates
    public void UpdateTimerDisplay(float currentTime)
    {
        if (timeText != null)
        {
            if(TimeManager.Instance.IsLastPhaseOver)
            {
                timeText.text = "나가야한다.";
                timeText.color = new Color(1f, 0f, 0f, 1f);
                return;
            }

            int minutes = Mathf.FloorToInt(currentTime / 60);
            int seconds = Mathf.FloorToInt(currentTime % 60);
            timeText.text = $"{minutes:00}:{seconds:00}";
        }
    }

    public void UpdatePhaseDisplay(int phase)
    {
        if (dayText != null)
        {
            if(TimeManager.Instance.IsLastPhaseOver)
            {
                dayText.text = "나가야한다.";
                dayText.color = new Color(1f, 0f, 0f, 1f);
                return;
            }

            dayText.text = $"{phase} 일차";
        }
    }

    // TimeManager에서 시간 업데이트 알림을 받음
    private void OnTimeUpdated(float currentTime)
    {
        UpdateTimerDisplay(currentTime); // 6분 15초 = 375초
    }

    // TimeManager에서 페이즈 변경 알림을 받음
    private void OnPhaseChanged(int phase)
    {
        UpdatePhaseDisplay(phase);
    }

    private void OnDayNightChanged(bool isDayLight)
    {
        SetCurrentAreaImage(isDayLight);
    }
    #endregion

    #region Area Image Change
    public void OnAreaChanged(AreaType areaType)
    {
        Debug.Log($"UIManager OnAreaChanged 실행");
        currentAreaText.text = AreaManager.AreaNameDictionary[areaType];
        SetCurrentAreaImage(areaType);
    }

    public void SetCurrentAreaImage(bool isDayLight)
    {
        Debug.Log($"UIManager SetCurrentAreaImage ver bool 실행");
        AreaType currentArea = AreaManager.Instance.PlayerCurrentArea.AreaType;

        SetAreaImageByType(currentArea, isDayLight);
    }

    public void SetCurrentAreaImage(AreaType area)
    {
        Debug.Log($"UIManager SetCurrentAreaImage ver AreaType 실행");

        bool isDayLight = TimeManager.Instance.IsDayTime;

        SetAreaImageByType(area, isDayLight);
    }

    private void SetAreaImageByType(AreaType area, bool isDayLight)
    {
        Debug.Log($"UIManager SetAreaImageByType 실행");
        if (!isImageLoadComplete)
        {
            Debug.LogWarning("UIManager: 이미지 아직 로딩 중...");
            return;
        }

        ApplyAreaImage(area, isDayLight);
    }

    // 이미지 적용 (로딩 완료 후)
    private void ApplyAreaImage(AreaType area, bool isDayLight)
    {
        Debug.Log($"UIManager ApplyAreaImage 실행");
        // ResourceManager에서 이미지 가져오기
        Sprite areaSprite = ResourceManager.Instance.GetAreaImage(area, isDayLight);

        // 실제 UI Image에 스프라이트 설정
        if (areaSprite != null && movementAreaImage != null && backgroundAreaImage != null)
        {
            movementAreaImage.sprite = areaSprite;
            backgroundAreaImage.sprite = areaSprite;
            Debug.Log($"UIManager: 이미지 적용 완료 - {area}");
        }
        else
        {
            Debug.LogWarning($"UIManager: 이미지 적용 실패 - {area}, {isDayLight}");
        }
        Debug.Log($"UIManager ApplyAreaImage 완료");
    }
    #endregion

    #region Player Movement UI
    public void PlayerMovement()
    {
        if (isMoving) return;

        SoundManager.Instance.PlaySFX(SoundManager.SFXType.Player_Footwalk);
        StartCoroutine(AreaUIMovementCoroutine());
    }

    // 현재 지역 UI를 위 아래로 이동하는 코루틴 (이동 애니메이션)
    public IEnumerator AreaUIMovementCoroutine()
    {
        isMoving = true;
        movementIcon.gameObject.SetActive(true);
        RectTransform areaRect = movementAreaImage.rectTransform;

        if (areaRect == null)
            Debug.LogError("UIManager: 이미지를 찾을 수 없습니다.");

        Vector2 originalPosition = areaRect.anchoredPosition;

        float moveDistance = 15f;
        float duration = 0.4f;

        // 이동 중 화면이 가까워지는 효과
        StartCoroutine(MoveScreenEffectCoroutine(duration * 3f));

        // 위 아래로 이동 후 원래 위치로 돌아옴
        yield return SmoothMoveAnchored(areaRect, new Vector2(originalPosition.x, originalPosition.y + moveDistance), duration);
        yield return SmoothMoveAnchored(areaRect, new Vector2(originalPosition.x, originalPosition.y - moveDistance), duration);
        yield return SmoothMoveAnchored(areaRect, originalPosition, duration);
        movementIcon.gameObject.SetActive(false);
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

    // 이동 중 화면이 가까워지는 효과
    private IEnumerator MoveScreenEffectCoroutine(float duration)
    {
        // movementAreaImage의 크기와 투명도를 1f로 설정
        movementAreaImage.rectTransform.localScale = new Vector2(1f, 1f);
        movementAreaImage.color = new Color(1f, 1f, 1f, 1f);
        // movementAreaImage의 크기를 1.2f로 점진적으로 변경
        Vector2 originalScale = movementAreaImage.rectTransform.localScale;
        Vector2 targetScale = new Vector2(1.2f, 1.2f);

        // 1초동안 1.2f로 점진적으로 변경
        float elapsed = 0f;
        while (elapsed < duration - 0.2f)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / (duration - 0.2f);
            movementAreaImage.rectTransform.localScale = Vector2.Lerp(originalScale, targetScale, t);
            yield return null;
        }
        movementAreaImage.rectTransform.localScale = targetScale;

        // 1.2f로 변경되면 0.2초동안 투명도를 0으로 점진적으로 변경
        elapsed = 0f;
        while (elapsed < 0.2f)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / 0.2f;
            movementAreaImage.color = new Color(1f, 1f, 1f, 1f - t);
            yield return null;
        }
        movementAreaImage.color = new Color(1f, 1f, 1f, 0f);
    }


    public void StartItemUseProgress(UsableItem item)
    {
        StartCoroutine(ItemUseProgressCoroutine(item));
    }

    private IEnumerator ItemUseProgressCoroutine(UsableItem item)
    {
        yield return ItemInteractingProgress();

        item.OnUse();
    }

    public void StartItemCraftingProgress()
    {
        StartCoroutine(ItemInteractingProgress());
    }

    // 플레이어가 아이템 제작 or 사용 시 노출되는 프로그레스 바
    private IEnumerator ItemInteractingProgress()
    {
        isProgress = true;
        float elapsed = 0f;

        while (elapsed < 3f)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / 3f;
            progressbarImage.fillAmount = t;

            yield return null;
        }

        progressbarImage.fillAmount = 0;
        isProgress = false;
    }

    public void UpdatePlayerProtectionText(int duration)
    {
        if (duration <= 0)
        {
            playerProtectionText.text = " ";
            return;
        }

        if (playerProtectionText != null)
        {
            playerProtectionText.text = $"부적 사용 중 : {duration}s..";
        }
    }
    #endregion

    private void SubscribeEvents()
    {
        // 이벤트 구독
        if (TimeManager.Instance != null)
        {
            TimeManager.Instance.OnTimeUpdated += OnTimeUpdated;
            TimeManager.Instance.OnPhaseChanged += OnPhaseChanged;
            TimeManager.Instance.OnDayNightChanged += OnDayNightChanged;
        }
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnAreaChanged += OnAreaChanged;
            GameManager.Instance.OnPlayerProtectionChanged += UpdatePlayerProtectionText;
            GameManager.Instance.OnGameVictory += ShowVictoryPopup;
            GameManager.Instance.OnGameDefeat += ShowDeathCausePopup;
        }
    }

    private void UnsubscribeEvents()
    {
        // 이벤트 구독 해제
        if (TimeManager.Instance != null)
        {
            TimeManager.Instance.OnTimeUpdated -= OnTimeUpdated;
            TimeManager.Instance.OnPhaseChanged -= OnPhaseChanged;
            TimeManager.Instance.OnDayNightChanged -= OnDayNightChanged;
        }
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnAreaChanged -= OnAreaChanged;
            GameManager.Instance.OnPlayerProtectionChanged -= UpdatePlayerProtectionText;
        }
    }

    private void OnDestroy()
    {
        UnsubscribeEvents();
        // ResourceManager가 메모리 관리를 담당하므로 여기서는 구독 해제만
    }
}
