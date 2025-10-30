using UnityEngine;
using System.Collections.Generic;
using System;
using Unity.VisualScripting;
using System.Linq;

public class ItemManager : Singleton<ItemManager>
{
    // 아이템 정보 관리
    [SerializeField] private List<ItemBaseSO> materialItemDataList = new List<ItemBaseSO>();
    [SerializeField] private List<UsableItem> usableItemDataList = new List<UsableItem>();
    public ItemBaseSO GetItemData(ItemType itemType) => materialItemDataList.Find(item => item.ItemType == itemType);
    public UsableItem GetUsableItemData(ItemType itemType) => usableItemDataList.Find(item => item.ItemType == itemType);
    public ItemBaseSO GetRandomItemData() => materialItemDataList[UnityEngine.Random.Range(0, materialItemDataList.Count)];
    public Dictionary<ItemType, string> ItemNameDictionary = new Dictionary<ItemType, string>()
    {
        {ItemType.Note, "쪽지"},
        {ItemType.Amulet, "부적"},
        {ItemType.RepairedRadio, "수리된 라디오"},
        {ItemType.Paper, "종이"},
        {ItemType.Pencil, "연필"},
        {ItemType.Brush, "브러시"},
        {ItemType.BrokenRadio, "고장난 라디오"},
        {ItemType.RepairKit, "수리 키트"},
    };

    // 아이템 조합 레시피
    [Serializable]
    public struct CraftingRecipe
    {
        public ItemType material1;
        public ItemType material2;
        public ItemType result;

        public CraftingRecipe(ItemType mat1, ItemType mat2, ItemType result)
        {
            this.material1 = mat1;
            this.material2 = mat2;
            this.result = result;
        }
    }

    [SerializeField]
    private List<CraftingRecipe> craftingRecipes = new List<CraftingRecipe>()
    {
        new CraftingRecipe(ItemType.Paper, ItemType.Pencil, ItemType.Note),
        new CraftingRecipe(ItemType.Paper, ItemType.Brush, ItemType.Amulet),
        new CraftingRecipe(ItemType.BrokenRadio, ItemType.RepairKit, ItemType.RepairedRadio),
    };

    private int selectedInventoryIndex = -1;
    public int SelectedInventoryIndex
    {
        get => selectedInventoryIndex;
        set
        {
            if (value < 0 || value >= currentInventory.Length)
                return;
            selectedInventoryIndex = value;
        }
    }

    // 현재 인벤토리 아이템 관리
    [SerializeField] private ItemBaseSO[] currentInventory = new ItemBaseSO[6];
    public bool IsInventoryFull => currentInventory.All(item => item != null);
    public Action OnInventoryItemChanged;

    private void Start()
    {

    }

    // 인벤토리 아이템 가져오기
    public ItemBaseSO GetInventoryItem(int index)
    {
        if (index < 0 || index >= currentInventory.Length)
            return null;
        return currentInventory[index];
    }

    // 인벤토리 전체 배열 가져오기 (UI 업데이트용)
    public ItemBaseSO[] GetInventoryItems() => currentInventory;

    public void AddItem(ItemBaseSO item)
    {
        if (IsInventoryFull)
        {
            UIManager.Instance.OnNoticeAdded?.Invoke(
                "인벤토리가 꽉 찼습니다.",
                NoticeType.Normal
            );
            return;
        }

        for (int i = 0; i < currentInventory.Length; i++)
        {
            if (currentInventory[i] != null) continue;
            currentInventory[i] = item.Clone();
            UIManager.Instance.OnNoticeAdded?.Invoke(
                $"{currentInventory[i].ItemName}을 얻었습니다.",
                NoticeType.Normal
            );
            break;
        }
        OnInventoryItemChanged?.Invoke();
    }

    public void RemoveItem(int index)
    {
        if (index < 0 || index >= currentInventory.Length || currentInventory[index] == null)
            return;

        currentInventory[index] = null;
        OnInventoryItemChanged?.Invoke();
    }

    // 특정 인덱스의 아이템으로 조합 가능한지 확인
    public bool IsCraftable(int itemIndex)
    {
        if (itemIndex < 0 || itemIndex >= currentInventory.Length || currentInventory[itemIndex] == null)
            return false;

        ItemType itemType = currentInventory[itemIndex].ItemType;

        foreach (var recipe in craftingRecipes)
        {
            if (recipe.material1 == itemType || recipe.material2 == itemType)
            {
                // 다른 재료가 인벤토리에 있는지 확인
                ItemType otherMaterial = (recipe.material1 == itemType) ? recipe.material2 : recipe.material1;
                if (HasItemInInventory(otherMaterial))
                {
                    return true;
                }
            }
        }
        return false;
    }

    // 특정 인덱스의 아이템으로 조합 결과 반환
    public ItemType? GetCraftingResult(int itemIndex)
    {
        if (itemIndex < 0 || itemIndex >= currentInventory.Length || currentInventory[itemIndex] == null)
            return null;

        ItemType itemType = currentInventory[itemIndex].ItemType;

        foreach (var recipe in craftingRecipes)
        {
            if (recipe.material1 == itemType || recipe.material2 == itemType)
            {
                ItemType otherMaterial = (recipe.material1 == itemType) ? recipe.material2 : recipe.material1;
                if (HasItemInInventory(otherMaterial))
                {
                    return recipe.result;
                }
            }
        }
        return null;
    }

    // 인벤토리에 아무 아이템이나 있는지 확인
    public bool HasItemAny()
    {
        foreach (var item in currentInventory)
        {
            if (item != null)
            {
                return true;
            }
        }
        return false;
    }

    // 인벤토리에 특정 아이템이 있는지 확인
    public bool HasItemInInventory(ItemType itemType)
    {
        foreach (var item in currentInventory)
        {
            if (item != null && item.ItemType == itemType)
            {
                return true;
            }
        }
        return false;
    }

    // 특정 인덱스의 아이템으로 조합 실행
    public bool TryCraftItem(int itemIndex)
    {
        if (itemIndex < 0 || itemIndex >= currentInventory.Length || currentInventory[itemIndex] == null)
            return false;

        var result = GetCraftingResult(itemIndex);
        if (result == null) return false;

        ItemType itemType = currentInventory[itemIndex].ItemType;

        // 첫 번째 재료 제거 (클릭한 아이템)
        currentInventory[itemIndex] = null;

        // 두 번째 재료 찾아서 제거
        var recipe = GetRecipeByMaterial(itemType);
        ItemType otherMaterial = (recipe.Value.material1 == itemType) ? recipe.Value.material2 : recipe.Value.material1;
        RemoveItemFromInventory(otherMaterial);

        // 결과 아이템 추가
        var resultItem = GetItemData(result.Value);
        AddItem(resultItem);

        OnInventoryItemChanged?.Invoke();
        return true;
    }

    // 인벤토리에서 특정 아이템 제거
    public void RemoveItemFromInventory(ItemType itemType)
    {
        for (int i = 0; i < currentInventory.Length; i++)
        {
            if (currentInventory[i] != null && currentInventory[i].ItemType == itemType)
            {
                currentInventory[i] = null;
                OnInventoryItemChanged?.Invoke();
                break;
            }
        }
    }

    // 인벤토리에서 아무 아이템이나 제거
    public void RemoveItemAny()
    {
        bool isRemoved = false;
        while(!isRemoved)
        {
            int randomIndex = UnityEngine.Random.Range(0, currentInventory.Length);
            if (currentInventory[randomIndex] != null)
            {
                currentInventory[randomIndex] = null;
                OnInventoryItemChanged?.Invoke();
                isRemoved = true;
            }
        }
    }

    // 재료로 레시피 찾기
    private CraftingRecipe? GetRecipeByMaterial(ItemType itemType)
    {
        foreach (var recipe in craftingRecipes)
        {
            if (recipe.material1 == itemType || recipe.material2 == itemType)
            {
                ItemType otherMaterial = (recipe.material1 == itemType) ? recipe.material2 : recipe.material1;
                if (HasItemInInventory(otherMaterial))
                {
                    return recipe;
                }
            }
        }
        return null;
    }

    // 특정 아이템으로 조합 가능한 레시피 리스트 반환
    public List<CraftingRecipe> GetAvailableCraftingRecipes(ItemType itemType)
    {
        List<CraftingRecipe> availableRecipes = new List<CraftingRecipe>();
        foreach (var recipe in craftingRecipes)
        {
            if (recipe.material1 == itemType || recipe.material2 == itemType)
            {
                ItemType otherMaterial = (recipe.material1 == itemType) ? recipe.material2 : recipe.material1;
                if (HasItemInInventory(otherMaterial) && !availableRecipes.Contains(recipe))
                {
                    availableRecipes.Add(recipe);
                }
            }
        }
        return availableRecipes;
    }

    // 특정 아이템으로 조합 실행
    public bool TryCraftItemWithRecipe(int itemIndex, CraftingRecipe recipe)
    {
        if (itemIndex < 0 || itemIndex >= currentInventory.Length || currentInventory[itemIndex] == null)
            return false;

        ItemType itemType = currentInventory[itemIndex].ItemType;

        // 레시피가 해당 아이템과 매치되는지 확인
        if (recipe.material1 != itemType && recipe.material2 != itemType)
            return false;

        ItemType otherMaterial = (recipe.material1 == itemType) ? recipe.material2 : recipe.material1;

        // 다른 재료가 인벤토리에 있는지 확인
        if (!HasItemInInventory(otherMaterial))
            return false;

        // 조합 실행
        currentInventory[itemIndex] = null;
        RemoveItemFromInventory(otherMaterial);

        UsableItem resultItem = GetUsableItemData(recipe.result);

        if(resultItem == null)
            return false;

        AddItem(resultItem);

        OnInventoryItemChanged?.Invoke();
        return true;
    }
}
