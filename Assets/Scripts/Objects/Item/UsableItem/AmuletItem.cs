using UnityEngine;

[CreateAssetMenu(fileName = "Amulet", menuName = "Item/Usable/Amulet", order = 0)]
public class AmuletItem : UsableItem
{
    public override void OnUse()
    {
        Debug.Log($"{ItemName}를 사용했습니다!");
        GameManager.Instance.StartPlayerProtection();
    }
}
