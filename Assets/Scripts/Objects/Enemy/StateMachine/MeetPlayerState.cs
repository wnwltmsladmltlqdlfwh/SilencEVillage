using UnityEngine;

public class MeetPlayerState : BaseState
{
    private float meetTimer = 0f;
    private bool isKillAnimationPlayed = false;

    public MeetPlayerState(EnemyBase enemy) : base(enemy)
    {
    }

    public override void Enter()
    {
        Debug.Log($"{enemy.EnemyData.EnemyName}이 플레이어와 만났습니다.");
        meetTimer = 0f;
        isKillAnimationPlayed = false;
    }

    public override void Exit()
    {
        Debug.Log($"{enemy.EnemyData.EnemyName}이 플레이어와 만남을 종료했습니다.");
        meetTimer = 0f;
        isKillAnimationPlayed = false;
    }

    public override void Update()
    {
        if (TimeManager.Instance.IsDayTime)
        {
            enemy.ChangeState(new IdleState(enemy));
            return;
        }

        if (enemy.CurrentArea.AreaType != AreaManager.Instance.PlayerCurrentArea.AreaType)
        {
            enemy.ChangeState(new MoveState(enemy));
            return;
        }

        meetTimer += Time.deltaTime;

        if (meetTimer >= enemy.EnemyData.MeetDelay && !isKillAnimationPlayed)
        {
            isKillAnimationPlayed = true;
            SoundManager.Instance.PlaySFX(enemy.EnemyData.EnemyType);
            EnemyManager.Instance.PlayKillAnimation(enemy.EnemyData.EnemyType, () =>
            {
                GameManager.Instance.GameDefeat(enemy.EnemyData);
            });
            return;
        }
    }
}