using UnityEngine;

[CreateAssetMenu(fileName = "Note", menuName = "Item/Usable/Note", order = 0)]
public class NoteItem : UsableItem
{
    private AreaType currentArea;

    public override void OnUse()
    {
        Debug.Log($"{ItemName}를 사용했습니다!");
        currentArea = AreaManager.Instance.PlayerCurrentArea.AreaType;
        AreaManager.Instance.GetAreaObject(currentArea).SetDirectionLureActive(true);
    }
}
