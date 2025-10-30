using UnityEngine;

[CreateAssetMenu(fileName = "Note", menuName = "Item/Usable/Note", order = 0)]
public class NoteItem : UsableItem
{
    private AreaType currentArea;

    public override void OnUse()
    {
        UIManager.Instance.OnNoticeAdded?.Invoke(
            $"{ItemName}을 사용했습니다!",
            NoticeType.System
        );
        currentArea = AreaManager.Instance.PlayerCurrentArea.AreaType;
        AreaManager.Instance.GetAreaObject(currentArea).SetDirectionLureActive(true);
    }
}
