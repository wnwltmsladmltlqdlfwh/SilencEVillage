using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class UseItemButton : MonoBehaviour, IPointerDownHandler
{
    // 지연 초기화를 위한 매니저 참조
    private UIManager _uiManager;
    private ItemManager _itemManager;

    // 프로퍼티로 안전한 접근
    private UIManager uiManager => _uiManager != null ? _uiManager : (_uiManager = UIManager.Instance);
    private ItemManager itemManager => _itemManager != null ? _itemManager : (_itemManager = ItemManager.Instance);

    [SerializeField] private UsableItem targetItem;
    [SerializeField] private TextMeshProUGUI itemDescription;

    public void InitUseItemButton(UsableItem itemData)
    {
        this.targetItem = itemData;
        itemDescription.text = $"{itemData.ItemName}\n사용";
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        OnClickUseButton();
    }

    public void OnClickUseButton()
    {
        if(uiManager.IsProgress) return;

        if(targetItem is RepairRadioItem && AreaManager.Instance.PlayerCurrentArea.IsSoundLureActive)
        {
            UIManager.Instance.OnNoticeAdded?.Invoke(
                "이미 라디오가 설치되어 있습니다.",
                NoticeType.System
            );
            return;
        }
        if(targetItem is NoteItem && AreaManager.Instance.PlayerCurrentArea.IsDirectionLureActive)
        {
            UIManager.Instance.OnNoticeAdded?.Invoke(
                "이미 노트가 설치되어 있습니다.",
                NoticeType.System
            );
            return;
        }
        if(targetItem is AmuletItem && GameManager.Instance.IsPlayerProtected)
        {
            UIManager.Instance.OnNoticeAdded?.Invoke(
                "이미 보호 상태입니다.",
                NoticeType.System
            );
            return;
        }

        SoundManager.Instance.PlaySFX(SoundManager.SFXType.Player_MakeItem);
        // 아이템 사용 진행 시작
        uiManager.StartItemUseProgress(targetItem);
        uiManager.CloseCraftingPopup();
        
        // 아이템 사용 후 인벤토리에서 제거
        itemManager.RemoveItem(itemManager.SelectedInventoryIndex);
    }
}
