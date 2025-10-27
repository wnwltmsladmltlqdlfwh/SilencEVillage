using UnityEngine;

public abstract class BaseState
{
    protected EnemyBase enemy;

    public BaseState(EnemyBase enemy)
    {
        this.enemy = enemy;
    }

    public abstract void Enter();
    public abstract void Exit();
    public abstract void Update();
    
    // Area 이동 관련 공통 메서드
    protected virtual void MoveToArea(AreaType targetAreaType)
    {
        if (enemy.CurrentArea.AreaType != targetAreaType)
        {
            enemy.MoveToArea(targetAreaType);
        }
    }
    
    // 현재 영역이 목표 영역인지 확인
    protected virtual bool IsInTargetArea(AreaBase targetArea)
    {
        return enemy.CurrentArea == targetArea;
    }

    // 플레이어 근처에 있는지 확인
    protected bool IsNearByPlayer()
    {
        var nearAreaList = AreaManager.MovableAreaDictionary[enemy.CurrentArea.AreaType];
        var playerArea = AreaManager.Instance.PlayerCurrentArea;
        if(nearAreaList.Contains(playerArea.AreaType))
        {
            enemy.OnNearByPlayer?.Invoke();
            return true;
        }
        else
            return false;
    }

    // 미끼 영역 확인
    protected bool OnLuredArea() => enemy.LuredAreaList.Count > 0 ? true : false;
}
