using UnityEngine;

[CreateAssetMenu(fileName = "RepairRadio", menuName = "Item/Usable/RepairRadio", order = 0)]
public class RepairRadioItem : UsableItem
{
    private AreaType currentArea;

    public override void OnUse()
    {
        UIManager.Instance.OnNoticeAdded?.Invoke(
            $"{ItemName}을 사용했습니다!",
            NoticeType.System
        );
        currentArea = AreaManager.Instance.PlayerCurrentArea.AreaType;
        AreaManager.Instance.GetAreaObject(currentArea).SetSoundLureActive(true);
        EnemyManager.Instance.ActivateSoundLure(currentArea);
    }
}
