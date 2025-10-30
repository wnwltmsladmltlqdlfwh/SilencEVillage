using System.Collections;
using UnityEngine;

public class JibakryeongBehavior : IEnemyBehavior
{
    private EnemyDataSO data;

    public JibakryeongBehavior(EnemyDataSO enemyData)
    {
        this.data = enemyData;
    }

    public void OnAreaEntered(AreaType areaType)
    {
        // 지박령은 이동하지 않는 몬스터이므로 플레이어 감지 체크만 진행
    }

    public void OnAreaExited(AreaType areaType)
    {
        // 지박령은 이동하지 않는 몬스터이므로 플레이어 감지 체크만 진행
    }
}