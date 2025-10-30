using UnityEngine;
using System.Collections.Generic;
using System;
public class AreaManager : Singleton<AreaManager>
{
    // 지역 관리 Dictionary
    private Dictionary<AreaType, AreaBase> areaDictionary = new Dictionary<AreaType, AreaBase>();
    public AreaBase GetAreaObject(AreaType area) => areaDictionary[area];

    // 플레이어 현재 영역
    private AreaType playerCurrentAreaType = AreaType.Entrance;
    public AreaBase PlayerCurrentArea
    {
        get
        {
            if (areaDictionary.ContainsKey(playerCurrentAreaType))
            {
                return areaDictionary[playerCurrentAreaType];
            }

            Debug.LogWarning($"AreaManager: {playerCurrentAreaType}가 Dictionary에 없음! Entrance로 설정");
            playerCurrentAreaType = AreaType.Entrance;

            if (!areaDictionary.ContainsKey(AreaType.Entrance))
            {
                areaDictionary[AreaType.Entrance] = new AreaBase(AreaType.Entrance);
            }

            return areaDictionary[AreaType.Entrance];
        }
    }

    // 지역 간 이동 가능한 지역 목록
    public static Dictionary<AreaType, List<AreaType>> MovableAreaDictionary = new Dictionary<AreaType, List<AreaType>>
    {
        {AreaType.Entrance,
            new List<AreaType> { AreaType.Crossroad, AreaType.VillageHeadHouse, AreaType.VillageHall }},
        {AreaType.VillageHall,
            new List<AreaType> { AreaType.Crossroad, AreaType.Cowshed, AreaType.Entrance }},
        {AreaType.Crossroad,
            new List<AreaType> { AreaType.Entrance, AreaType.Cowshed, AreaType.VillageHall, AreaType.HolyTree, AreaType.VillageHeadHouse }},
        {AreaType.Cowshed,
            new List<AreaType> { AreaType.Crossroad, AreaType.HolyTree, AreaType.VillageHall, AreaType.Greenhouse }},
        {AreaType.Greenhouse,
            new List<AreaType> { AreaType.Cowshed, AreaType.HolyTree }},
        {AreaType.HolyTree,
            new List<AreaType> { AreaType.Cowshed, AreaType.Greenhouse, AreaType.Crossroad, AreaType.VillageHeadHouse }},
        {AreaType.VillageHeadHouse,
            new List<AreaType> { AreaType.Crossroad, AreaType.Entrance, AreaType.HolyTree }}
    };

    // 지역 이름 딕셔너리
    public static Dictionary<AreaType, string> AreaNameDictionary = new Dictionary<AreaType, string>()
    {
        {AreaType.Entrance, "마을 입구"},
        {AreaType.Crossroad, "갈림길"},
        {AreaType.VillageHall, "마을회관"},
        {AreaType.VillageHeadHouse, "이장의집"},
        {AreaType.HolyTree, "서낭당"},
        {AreaType.Cowshed, "외양간"},
        {AreaType.Greenhouse, "비닐하우스"},
    };

    // 이동 가능 여부 확인
    public static List<AreaType> GetMovableAreaList(AreaType area) => MovableAreaDictionary[area];
    public bool IsMovable(AreaType fromArea, AreaType toArea) => GetMovableAreaList(fromArea).Contains(toArea);

    protected override void Awake()
    {
        base.Awake();

        if (Instance == this)
        {
            InitializeAreaDictionary();
        }
    }

    private void InitializeAreaDictionary()
    {
        // 지역 정보 초기화
        areaDictionary.Clear();

        foreach (var areaType in Enum.GetValues(typeof(AreaType)))
        {
            AreaType type = (AreaType)areaType;

            // AreaMaxCount 제외
            if (type == AreaType.AreaMaxCount) continue;

            if (!areaDictionary.ContainsKey(type))
            {
                areaDictionary.Add(type, new AreaBase(type));
            }
        }

        Debug.Log($"AreaManager: Dictionary 초기화 완료 ({areaDictionary.Count}개 지역)");
    }

    public void InitAreaManager()
    {
        // 플레이어 현재 영역 설정
        if (playerCurrentAreaType != AreaType.Entrance)
            SetPlayerArea(AreaType.Entrance);

        Debug.Log("AreaManager: 초기화 완료");
    }

    public void SetPlayerArea(AreaType areaType)
    {
        playerCurrentAreaType = areaType;

        if(playerCurrentAreaType == AreaType.Entrance && TimeManager.Instance.IsLastPhaseOver)
        {
            GameManager.Instance.GameVictory();
        }
    }

    // Enemy 이동 길찾기 알고리즘 (BFS 사용)
    public List<AreaType> FindPath(AreaType start, AreaType target)
    {
        if (start == target) return new List<AreaType> { start };

        Queue<AreaType> queue = new Queue<AreaType>();  // 탐색할 AreaType Queue
        Dictionary<AreaType, AreaType> parent = new Dictionary<AreaType, AreaType>();  // 이어진 길을 저장하는 Dict
        HashSet<AreaType> visited = new HashSet<AreaType>();  // 방문한 AreaType 집합

        queue.Enqueue(start);
        visited.Add(start);

        while (queue.Count > 0)
        {
            AreaType current = queue.Dequeue();  // 현재 탐색할 AreaType

            if (current == target)  // 현재 탐색할 AreaType가 목표 AreaType와 같으면 경로 재구성 시작
            {
                List<AreaType> path = new List<AreaType>(); // 경로 저장 리스트
                AreaType node = target; // 경로 재구성 시 역추적하고 있는 현재 위치

                while (node != start)  // 현재 위치가 시작 AreaType가 될 때까지 역추적
                {
                    path.Add(node);  // 경로 저장 리스트에 현재 위치 추가
                    node = parent[node];  // 이전 위치로 이동
                }
                path.Add(start);  // 시작 AreaType 추가
                path.Reverse();  // 경로 리스트 역순으로 변경

                return path;  // 경로 리스트 반환
            }

            // 인접한 영역들 확인
            if (MovableAreaDictionary.ContainsKey(current))  // 현재 탐색할 AreaType의 인접한 영역들 확인
            {
                foreach (AreaType neighbor in MovableAreaDictionary[current])
                {
                    if (!visited.Contains(neighbor))  // 방문한 AreaType 집합에 현재 탐색할 AreaType의 인접한 영역이 없으면 탐색
                    {
                        visited.Add(neighbor);  // 방문한 AreaType 집합에 현재 탐색할 AreaType의 인접한 영역 추가
                        parent[neighbor] = current;  // 이전 위치로 이동
                        queue.Enqueue(neighbor);  // 탐색할 AreaType 큐에 현재 탐색할 AreaType의 인접한 영역 추가
                    }
                }
            }
        }

        // 경로를 찾을 수 없는 경우
        return new List<AreaType>();
    }

    public void ResetAllAreaEnemyList()
    {
        foreach (var area in areaDictionary)
        {
            area.Value.ClearEnemyList();
        }
        Debug.Log("AreaManager: 모든 지역의 적 리스트 초기화 완료");
    }
}
