using UnityEngine;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;


[CreateAssetMenu(fileName = "AreaBase", menuName = "AreaBase", order = 0)]
public class AreaBase : ScriptableObject
{
    // 영역 정보
    [SerializeField] private AreaType areaType;
    public AreaType AreaType => areaType;
    public string ObjectName => $"Area_{areaType.ToString()}";


    // 설치 오브젝트
    private bool isSoundLureActive = false;
    public bool IsSoundLureActive
    {
        get { return isSoundLureActive; }
    }

    private bool isDirectionLureActive = false;
    public bool IsDirectionLureActive
    {
        get { return isDirectionLureActive; }
    }

    // 적
    private List<EnemyBase> enemyObjectList = new List<EnemyBase>();

    public void InitArea()
    {
        enemyObjectList.Clear();
        isSoundLureActive = false;
        isDirectionLureActive = false;
    }


    public void AddEnemy(EnemyBase enemy) => enemyObjectList.Add(enemy);
    public void RemoveEnemy(EnemyBase enemy) { if (enemyObjectList.Contains(enemy)) enemyObjectList.Remove(enemy); }

    public void SetSoundLureActive(bool isActive)
    {
        isSoundLureActive = isActive;
    }
    
    public void SetDirectionLureActive(bool isActive)
    {
        isDirectionLureActive = isActive;
    }
}
