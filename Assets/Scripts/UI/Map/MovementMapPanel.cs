using UnityEngine;
using UnityEngine.UI;

public class MovementMapPanel : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private Button closeButton;
    [SerializeField] private MovementMapButton[] movementMapButtons = new MovementMapButton[(int)AreaType.AreaMaxCount];

    private void Awake()
    {
        GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -1100f);
        animator = GetComponent<Animator>();
        closeButton.onClick.AddListener(CloseMovementMapPanel);
        foreach (var button in GetComponentsInChildren<MovementMapButton>())
        {
            if(movementMapButtons[(int)button.AreaType] != null)
            {
                Debug.LogError($"MovementMapButton[{button.AreaType}]에 할당된 버튼이 이미 존재합니다.");
                continue;
            }
            movementMapButtons[(int)button.AreaType] = button;
        }
    }

    private void OnEnable()
    {
        // 이벤트 구독
        MovementMapButton.OnAreaMovementStarted += CloseMovementMapPanel;
    }

    private void OnDisable()
    {
        // 이벤트 구독 해제
        MovementMapButton.OnAreaMovementStarted -= CloseMovementMapPanel;
    }

    public void OpenMovementMapPanel()
    {
        animator.SetTrigger("MapMoveUp");
        GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0f);
        foreach (var button in movementMapButtons)
        {
            button.SetupInfoTheArea();
        }
    }

    public void CloseMovementMapPanel()
    {
        animator.SetTrigger("MapMoveDown");
        GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -1100f);
    }
}
