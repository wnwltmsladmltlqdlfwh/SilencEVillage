using UnityEngine;

public class MeetPlayerState : BaseState
{
    public MeetPlayerState(EnemyBase enemy) : base(enemy)
    {
    }

    public override void Enter()
    {
        Debug.Log($"{enemy.EnemyData.EnemyName}이 플레이어와 만났습니다.");
    }

    public override void Exit()
    {
        Debug.Log($"{enemy.EnemyData.EnemyName}이 플레이어와 만남을 종료했습니다.");
    }

    public override void Update()
    {
        if(TimeManager.Instance.IsDayTime)
        {
            enemy.ChangeState(new IdleState(enemy));
            return;
        }

        if(enemy.CurrentArea == AreaManager.Instance.PlayerCurrentArea)
        {
            GameManager.Instance.GameDefeat();
        }
    }
}