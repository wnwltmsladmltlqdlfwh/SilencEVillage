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
    public Action<AreaType> OnAreaEntered;
    public Action<AreaType> OnAreaExited;

    // 상태패턴 머신
    private FSM fsm;
    private Dictionary<EnemyState, BaseState> stateDict = new Dictionary<EnemyState, BaseState>();

    // Behavior 시스템
    private IEnemyBehavior enemyBehavior;

    public void InitEnemyData()
    {
        // 초기 상태 설정 (예: InactiveState)
        enemyData = enemyData.Clone();

        InitStateMachine();
        InitBehavior();
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
            // 이전 Area에서 제거 (지역 이탈 이벤트 발생)
            CurrentArea.RemoveEnemy(this);
            OnAreaExited?.Invoke(currentAreaType);

            // 새 Area로 이동 (지역 진입 이벤트 발생)
            currentAreaType = targetAreaType;
            CurrentArea.AddEnemy(this);
            OnAreaEntered?.Invoke(targetAreaType);

            // 이동한 영역이 플레이어 영역과 연결되어 있으면 경고 알림 추가
            if (AreaManager.Instance.IsMovable(targetAreaType, AreaManager.Instance.PlayerCurrentArea.AreaType))
            {
                UIManager.Instance.OnNoticeAdded?.Invoke(
                $"{AreaManager.AreaNameDictionary[targetAreaType]}에서 인기척이 느껴집니다.",
                NoticeType.Warning
                );
            }
        }
    }

    // FSM 접근을 위한 메서드
    public void ChangeState(BaseState newState)
    {
        fsm.ChangeState(newState);
    }

    public void AddLuredArea(AreaType targetAreaType)
    {
        if (!LuredAreaList.Contains(targetAreaType))
        {
            LuredAreaList.Add(targetAreaType);
        }

        // 성주는 미끼 영역 이동 시 이동 속도 증가
        if (enemyData.EnemyType == EnemyType.Seongju)
        {
            float moveFastDelay = enemyData.MoveDelay * 0.5f;
            enemyData.MoveDelay = moveFastDelay;
        }
    }

    public void RemoveLuredArea(AreaType targetAreaType)
    {
        if (LuredAreaList.Contains(targetAreaType))
            LuredAreaList.Remove(targetAreaType);

        // 성주는 미끼 영역이 없으면 원래 이동속도로 복귀
        if (enemyData.EnemyType == EnemyType.Seongju && LuredAreaList.Count <= 0)
        {
            float moveSlowDelay = enemyData.MoveDelay * 2f;
            enemyData.MoveDelay = moveSlowDelay;
        }
    }

    // Behavior 시스템 초기화
    private void InitBehavior()
    {
        switch (enemyData.EnemyType)
        {
            case EnemyType.Jangsanbeom:
                enemyBehavior = new JangsanbeomBehavior(enemyData);
                break;
            case EnemyType.Seongju:
                enemyBehavior = new SeongjuBehavior(enemyData);
                break;
            case EnemyType.Jibakryeong:
                enemyBehavior = new JibakryeongBehavior(enemyData);
                break;
            case EnemyType.Agwi:
                enemyBehavior = new AgwiBehavior(enemyData);
                break;
        }

        OnAreaEntered += TriggerAreaEntered;
        OnAreaExited += TriggerAreaExited;
    }

    // Behavior 메서드들

    public void TriggerAreaEntered(AreaType areaType)
    {
        enemyBehavior?.OnAreaEntered(areaType);
    }

    public void TriggerAreaExited(AreaType areaType)
    {
        enemyBehavior?.OnAreaExited(areaType);
    }

    private void OnDisable()
    {
        fsm.ChangeState(stateDict[EnemyState.Idle]);
    }
}
