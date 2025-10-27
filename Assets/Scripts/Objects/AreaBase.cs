using UnityEngine;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;

public class AreaBase
{
    // 영역 정보
    public AreaType AreaType { get; private set; }
    public string AreaName;


    // 설치 오브젝트
    private bool isSoundLureActive = false;
    private bool isDirectionLureActive = false;
    public bool IsSoundLureActive => isSoundLureActive;
    public bool IsDirectionLureActive => isDirectionLureActive;

    // 적 정보 관리
    private List<EnemyBase> enemyObjectList = new List<EnemyBase>();
    public int EnemyObjectCount => enemyObjectList.Count;


    public AreaBase(AreaType areaType)
    {
        this.AreaType = areaType;
        this.AreaName = areaType.ToString();
        
        enemyObjectList.Clear();
        isSoundLureActive = false;
        isDirectionLureActive = false;
    }

    public void AddEnemy(EnemyBase enemy) => enemyObjectList.Add(enemy);
    public void RemoveEnemy(EnemyBase enemy) { if (enemyObjectList.Contains(enemy)) enemyObjectList.Remove(enemy); }

    public void SetSoundLureActive(bool isActive)
    {
        isSoundLureActive = isActive;
        Debug.Log($"Sound Lure Active: {isActive}");
    }
    
    public void SetDirectionLureActive(bool isActive)
    {
        isDirectionLureActive = isActive;
        Debug.Log($"Direction Lure Active: {isActive}");
    }
}
