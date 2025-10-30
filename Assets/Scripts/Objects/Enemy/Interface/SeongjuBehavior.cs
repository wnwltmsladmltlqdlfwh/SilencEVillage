using UnityEngine;

public class SeongjuBehavior : IEnemyBehavior
{
    private EnemyDataSO data;
    
    public SeongjuBehavior(EnemyDataSO enemyData)
    {
        this.data = enemyData;
    }

    public void OnAreaEntered(AreaType areaType)
    {
        // 지역 진입 시 로직
        Debug.Log($"{data.EnemyName}이 {areaType} 지역에 진입했습니다.");
    }
    
    public void OnAreaExited(AreaType areaType)
    {
        // 지역 이탈 시 로직
        Debug.Log($"{data.EnemyName}이 {areaType} 지역에서 이탈했습니다.");
    }
}