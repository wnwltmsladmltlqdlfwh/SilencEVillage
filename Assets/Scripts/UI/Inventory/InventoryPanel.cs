using UnityEngine;

public class InventoryPanel : MonoBehaviour
{
    [SerializeField] private Slot[] slots;

    private void Start()
    {
        ItemManager.Instance.OnInventoryItemChanged += UpdateInventory;
        UpdateInventory();
    }

    public void UpdateInventory()
    {
        ItemBaseSO[] inventoryItems = ItemManager.Instance.GetInventoryItems();
        for(int i = 0; i < slots.Length; i++)
        {
            if(inventoryItems[i] != null)
                slots[i].SetItem(inventoryItems[i], i);
            else
                slots[i].ClearSlot();
        }
    }

    private void OnDestroy()
    {
        ItemManager.Instance.OnInventoryItemChanged -= UpdateInventory;
    }
}
