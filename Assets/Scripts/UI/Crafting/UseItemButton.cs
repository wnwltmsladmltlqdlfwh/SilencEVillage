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
        itemDescription.text = $"{itemData.ItemName.ToString()}";
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        OnClickUseButton();
    }

    public void OnClickUseButton()
    {
        // 아이템 사용 진행 시작
        uiManager.StartItemUseProgress(targetItem);
        uiManager.CloseCraftingPopup();
        
        // 아이템 사용 후 인벤토리에서 제거
        itemManager.RemoveItem(itemManager.SelectedInventoryIndex);
    }
}
