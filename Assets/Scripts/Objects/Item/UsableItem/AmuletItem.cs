using UnityEngine;

[CreateAssetMenu(fileName = "Amulet", menuName = "Item/Usable/Amulet", order = 0)]
public class AmuletItem : UsableItem
{
    public override void OnUse()
    {
        UIManager.Instance.OnNoticeAdded?.Invoke(
            $"{ItemName}을 사용했습니다!",
            NoticeType.System
        );
        GameManager.Instance.StartPlayerProtection();
    }
}
