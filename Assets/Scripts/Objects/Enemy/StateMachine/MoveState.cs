using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class MoveState : BaseState
{
    private AreaBase playerArea;
    private float chaseTimer;

    public MoveState(EnemyBase enemy) : base(enemy)
    {
    }

    public override void Enter()
    {
        Debug.Log($"{enemy.EnemyData.EnemyName}이 움직이기 시작했습니다.");
        playerArea = AreaManager.Instance.PlayerCurrentArea;
        chaseTimer = 0f;
    }

    public override void Exit()
    {
        Debug.Log($"{enemy.EnemyData.EnemyName}이 움직임을 중단했습니다.");
        playerArea = null;
        chaseTimer = 0f;
    }

    public override void Update()
    {
        chaseTimer += Time.deltaTime;

        if (TimeManager.Instance.IsDayTime)
        {
            enemy.ChangeState(new IdleState(enemy));
            return;
        }

        if (chaseTimer >= enemy.EnemyData.MoveDelay)
        {
            // 플레이어와 같은 영역에 있으면 만남 상태로 전환
            if (enemy.CurrentArea == playerArea)
            {
                enemy.ChangeState(new MeetPlayerState(enemy));
                return;
            }

            EnemyMovement();

            // 방향 미끼가 활성화 시 미끼 리스트 추가
            if (enemy.CurrentArea.IsDirectionLureActive)
            {
                enemy.AddLuredArea(AreaType.Entrance);
                enemy.CurrentArea.SetDirectionLureActive(false);
            }
        }
    }

    public void EnemyMovement()
    {
        float randomChance = 0.2f * TimeManager.Instance.CurrentPhase;

        if (Random.Range(0f, 1f) >= randomChance)
        {
            chaseTimer = 0f;
            return;
        }

        bool isChase = enemy.EnemyData.isChasePlayer;

        if (IsNearByPlayer())   // 플레이어가 1칸 이내에 있으면 플레이어 영역으로 이동
        {
            MoveToArea(AreaManager.Instance.PlayerCurrentArea.AreaType);
            Debug.Log($"{enemy.EnemyData.EnemyName}이 플레이어와 가까워 {AreaManager.Instance.PlayerCurrentArea.AreaType} 영역으로 이동했습니다.");
            chaseTimer = 0f;
            return;
        }

        if (OnLuredArea())   // 미끼 영역 리스트가 있으면 가장 첫번째 미끼 영역으로 이동
        {
            Debug.Log($"{enemy.EnemyData.EnemyName}이 미끼 영역인 {enemy.LuredAreaList[0]} 영역으로 이동했습니다.");
            LuredMove();
        }

        if (isChase)   // 추격 모드
        {
            ChaseMove();
            Debug.Log($"{enemy.EnemyData.EnemyName}이 플레이어를 추격하여 {AreaManager.Instance.PlayerCurrentArea.AreaType} 영역으로 이동했습니다.");
        }
        else   // 랜덤 목표 이동 모드
        {
            RandomMove();
            Debug.Log($"{enemy.EnemyData.EnemyName}이 임의로 {enemy.CurrentArea.AreaType} 영역으로 이동했습니다.");
        }

        chaseTimer = 0f;
    }

    public void LuredMove()    // 미끼 영역 이동 모드
    {
        AreaType luredAreaType = enemy.LuredAreaList[0];
        AreaBase currentArea = enemy.CurrentArea;
        AreaBase targetArea = AreaManager.Instance.GetAreaObject(luredAreaType);

        // 이미 목표에 도착했는지 확인
        if (currentArea.AreaType == luredAreaType)
        {
            // 도착 처리
            if (targetArea.IsSoundLureActive)
            {
                EnemyManager.Instance.DeactivateSoundLure(luredAreaType);
                targetArea.SetSoundLureActive(false);
            }
            else
            {
                enemy.RemoveLuredArea(luredAreaType);
            }
            chaseTimer = 0f;
            return;
        }

        // 경로 찾기 (ChaseMove와 동일한 로직)
        List<AreaType> pathToLure = AreaManager.Instance.FindPath(currentArea.AreaType, luredAreaType);

        if (pathToLure == null || pathToLure.Count < 2)
        {
            // 경로를 찾을 수 없으면 LureList에서 제거
            Debug.LogWarning($"{enemy.EnemyData.EnemyName}: {luredAreaType}로 가는 경로를 찾을 수 없습니다!");
            enemy.RemoveLuredArea(luredAreaType);
            chaseTimer = 0f;
            return;
        }

        // 한 칸씩 이동
        AreaType nextArea = pathToLure[1];
        MoveToArea(nextArea);

        chaseTimer = 0f;
        return;
    }

    public void ChaseMove()    // 추격 모드
    {
        AreaBase playerArea = AreaManager.Instance.PlayerCurrentArea;
        AreaBase currentArea = enemy.CurrentArea;

        if (playerArea == currentArea)
        {
            // 플레이어와 같은 영역에 있으면 만남 상태로 전환
            enemy.ChangeState(new MeetPlayerState(enemy));
            return;
        }

        List<AreaType> pathToPlayer = AreaManager.Instance.FindPath(currentArea.AreaType, playerArea.AreaType);   // 플레이어 영역까지의 경로

        if (pathToPlayer == null || pathToPlayer.Count < 2)
        {
            enemy.ChangeState(new MeetPlayerState(enemy));
            return;
        }

        AreaType nextArea = pathToPlayer[1];
        MoveToArea(nextArea);
    }

    public void RandomMove()    // 랜덤 목표 이동 모드
    {
        List<AreaType> moveableArea = AreaManager.GetMovableAreaList(enemy.CurrentArea.AreaType);   // 이동 가능한 영역 리스트

        if (moveableArea == null || moveableArea.Count == 0)
        {
            return;
        }

        int randomIndex = Random.Range(0, moveableArea.Count);   // 이동 가능한 영역 중 랜덤으로 선택

        MoveToArea(moveableArea[randomIndex]);   // 선택한 영역으로 이동
    }
}