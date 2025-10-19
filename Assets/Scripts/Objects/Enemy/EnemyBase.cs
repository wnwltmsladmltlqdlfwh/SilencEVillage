using UnityEngine;
using System.Collections.Generic;
using System;

public enum EnemyState
{
    Idle,   // 낮 시간에는 비활성화
    Move,  // 플레이어 추격 패턴
    MeetPlayer,  // 플레이어와 만남 패턴
}

public class EnemyBase : MonoBehaviour
{
    [SerializeField] private EnemyDataSO enemyData;
    public EnemyDataSO EnemyData => enemyData;
    
    [SerializeField] private AreaType currentAreaType;
    public AreaBase CurrentArea => AreaManager.Instance.GetAreaObject(currentAreaType);

    public List<AreaType> LuredAreaList = new List<AreaType>();

    // 몬스터가 플레이어 영역 근처에 있을 때 발생하는 이벤트
    public Action OnNearByPlayer;

    // 상태패턴 머신
    private FSM fsm;
    private Dictionary<EnemyState, BaseState> stateDict = new Dictionary<EnemyState, BaseState>();

    public void InitEnemyData()
    {
        // 초기 상태 설정 (예: InactiveState)
        enemyData = enemyData.Clone();

        InitStateMachine();
    }

    public void InitStateMachine()
    {
        IdleState inactiveState = new IdleState(this);
        MoveState chasingPlayerState = new MoveState(this);
        MeetPlayerState meetingPlayerState = new MeetPlayerState(this);

        stateDict.Add(EnemyState.Idle, inactiveState);
        stateDict.Add(EnemyState.Move, chasingPlayerState);
        stateDict.Add(EnemyState.MeetPlayer, meetingPlayerState);

        fsm = new FSM(inactiveState);
    }

    private void Update()
    {
        fsm?.UpdateState();
    }
    
    // Area 이동 메서드
    public void MoveToArea(AreaType targetAreaType)
    {
        if (currentAreaType != targetAreaType)
        {
            // 이전 Area에서 제거
            CurrentArea.RemoveEnemy(this);
            
            // 새 Area로 이동
            currentAreaType = targetAreaType;
            CurrentArea.AddEnemy(this);
            
            // Debug.Log($"{enemyData.EnemyName}이 {AreaManager.Instance.GetAreaObject(targetAreaType).ObjectName}로 이동했습니다.");
        }
    }
    
    // FSM 접근을 위한 메서드
    public void ChangeState(BaseState newState)
    {
        fsm.ChangeState(newState);
    }

    public void AddLuredArea(AreaType targetAreaType)
    {
        if(!LuredAreaList.Contains(targetAreaType))
            LuredAreaList.Add(targetAreaType);
    }

    public void RemoveLuredArea(AreaType targetAreaType)
    {
        if(LuredAreaList.Contains(targetAreaType))
            LuredAreaList.Remove(targetAreaType);
    }
}
