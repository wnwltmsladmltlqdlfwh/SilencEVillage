using UnityEngine;

public class IdleState : BaseState
{
    public IdleState(EnemyBase enemy) : base(enemy)
    {
    }

    public override void Enter()
    {
        Debug.Log($"{enemy.EnemyData.EnemyName}이 비활성화 상태가 되었습니다.");
        // 낮 시간 동안 비활성화
    }

    public override void Exit()
    {
        Debug.Log($"{enemy.EnemyData.EnemyName}이 비활성화 상태를 벗어났습니다.");
    }

    public override void Update()
    {
        // 밤이 되면 활성화 상태로 전환
        if (!TimeManager.Instance.IsDayTime)
        {
            enemy.ChangeState(new MoveState(enemy));
        }
    }
}
