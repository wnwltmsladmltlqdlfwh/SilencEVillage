using UnityEngine;
    
public class AgwiBehavior : IEnemyBehavior
{
    private EnemyDataSO data;
    private ItemManager itemManager;

    public AgwiBehavior(EnemyDataSO enemyData)
    {
        this.data = enemyData;
        itemManager = ItemManager.Instance;
    }
    
    public void OnAreaEntered(AreaType areaType)
    {
        // 지역 진입 시 로직
        Debug.Log($"{data.EnemyName}이 {areaType} 지역에 진입했습니다.");
        if(GameManager.Instance.IsPlayerProtected) return;

        if (EnemyManager.Instance.IsNearByPlayer(data.EnemyType) || areaType == AreaManager.Instance.PlayerCurrentArea.AreaType)
        {
            if(itemManager.HasItemAny())
            {
                itemManager.RemoveItemAny();
                // TODO. 텍스트 알림 표시
            }
        }
    }
    
    public void OnAreaExited(AreaType areaType)
    {
        // 지역 이탈 시 로직
        Debug.Log($"{data.EnemyName}이 {areaType} 지역에서 이탈했습니다.");
    }
}