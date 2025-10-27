using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using System;

public class MovementMapButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    // 이벤트 정의
    public static event Action OnAreaMovementStarted;
    
    // 해당 지역의 정보
    [SerializeField] private AreaType areaType;
    public AreaType AreaType => areaType;
    [SerializeField] private TextMeshProUGUI areaNameText;
    [SerializeField] private TextMeshProUGUI areaEnemyCountText;
    
    // 지연 초기화를 위한 매니저 참조
    private AreaManager _areaManager;
    private UIManager _uiManager;
    
    // 프로퍼티로 안전한 접근
    private AreaManager areaManager => _areaManager != null ? _areaManager : (_areaManager = AreaManager.Instance);
    private UIManager uiManager => _uiManager != null ? _uiManager : (_uiManager = UIManager.Instance);

    // 선택 상태 관리
    [SerializeField] private Outline outline;
    [SerializeField] private Color onPointerEnterColor;
    [SerializeField] private Color unableToMoveColor;
    [SerializeField] private Color normalColor;
    [SerializeField] private Color currentColor;

    private void Awake()
    {
        outline = GetComponent<Outline>();

        if (areaNameText != null)
        {
            areaNameText.text = areaType.ToString();
        }

        onPointerEnterColor = new Color(0f, 1f, 0f, 0.5f);
        unableToMoveColor = new Color(1f, 0f, 0f, 0.5f);
        normalColor = new Color(0f, 0f, 0f, 0.5f);
        currentColor = new Color(0f, 1f, 0f, 1f);
    }

    public void SetupInfoTheArea()
    {
        AreaType currentArea = areaManager.PlayerCurrentArea.AreaType;
        if (areaManager.IsMovable(currentArea, areaType))
        {
            outline.effectColor = normalColor;
        }
        else if (currentArea == areaType)
        {
            outline.effectColor = currentColor;
        }
        else
        {
            outline.effectColor = unableToMoveColor;
        }

        if(areaManager.GetAreaObject(areaType).EnemyObjectCount > 0)
            areaEnemyCountText.text = $"{areaManager.GetAreaObject(areaType).EnemyObjectCount}";
        else
            areaEnemyCountText.text = " ";
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        AreaType currentArea = areaManager.PlayerCurrentArea.AreaType;
        if (!areaManager.IsMovable(currentArea, areaType) || uiManager.IsMoving) return;

        outline.effectColor = onPointerEnterColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        AreaType currentArea = areaManager.PlayerCurrentArea.AreaType;
        if (!areaManager.IsMovable(currentArea, areaType) || uiManager.IsMoving) return;

        outline.effectColor = normalColor;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        AreaType currentArea = areaManager.PlayerCurrentArea.AreaType;
        if (!areaManager.IsMovable(currentArea, areaType) || uiManager.IsMoving) return;

        MoveToOtherArea();
    }

    public void MoveToOtherArea()
    {
        // 이벤트 발생으로 패널 닫기 요청
        OnAreaMovementStarted?.Invoke();
        
        // 지연 초기화로 안전한 접근
        uiManager.PlayerMovement();
        areaManager.SetPlayerArea(areaType);
        GameManager.Instance.OnAreaChanged?.Invoke(areaType);
    }
}
