using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;

public class EnemyManager : Singleton<EnemyManager>
{
    [Header("Enemy List")]
    [SerializeField] private List<EnemyBase> enemyList = new List<EnemyBase>();

    [SerializeField] private GameObject deathParentObject;
    [SerializeField] private Animator deathAnimator;

    public void InitEnemyManager()
    {
        foreach (var enemy in enemyList)
        {
            enemy.InitEnemyData();
        }
        
        TimeManager.Instance.OnDayNightChanged += OnDayNightChanged;
        OnDayNightChanged(TimeManager.Instance.IsDayTime);

        if(deathParentObject != null && deathAnimator != null)
        {
            deathParentObject.SetActive(false);
            deathAnimator.gameObject.SetActive(false);
        }

        Debug.Log("EnemyManager: 초기화 완료");
    }

    public void OnDayNightChanged(bool isDayLight)
    {
        if(isDayLight)
        {
            EnemyDespawn();
            AreaManager.Instance.ResetAllAreaEnemyList();
            return;
        }

        List<EnemyBase> inactiveEnemyList = new List<EnemyBase>();
        foreach (var enemy in enemyList)
        {
            if(!enemy.gameObject.activeSelf)
            {
                inactiveEnemyList.Add(enemy);
            }
        }
        Debug.Log($"EnemyManager: 비활성화 적 리스트 초기화 완료 ({inactiveEnemyList.Count}개 적)");

        int spawnCount = Mathf.Min(TimeManager.Instance.CurrentPhase, inactiveEnemyList.Count);

        Debug.Log($"EnemyManager: {spawnCount}만큼 적 생성");

        // 현재 페이즈만큼 적 생성
        // 중복 생성 방지
        for (int i = 0; i < spawnCount; i++)
        {
            int randomIndex = UnityEngine.Random.Range(0, inactiveEnemyList.Count);
            EnemyBase randomEnemy = inactiveEnemyList[randomIndex];

            randomEnemy.gameObject.SetActive(true);
            randomEnemy.CurrentArea.AddEnemy(randomEnemy);
            int randomAreaIndex = UnityEngine.Random.Range(0, (int)AreaType.AreaMaxCount);
            randomEnemy.MoveToArea((AreaType)randomAreaIndex);

            inactiveEnemyList.RemoveAt(randomIndex);
        }
    }

    public void EnemyDespawn()
    {
        foreach (var enemy in enemyList)
        {
            if(!enemy.gameObject.activeSelf) continue;
            enemy.gameObject.SetActive(false);
        }
    }

    public void ActivateSoundLure(AreaType targetAreaType)
    {
        foreach (var enemy in enemyList)
        {
            enemy.AddLuredArea(targetAreaType);
        }
    }

    public void DeactivateSoundLure(AreaType targetAreaType)
    {
        foreach (var enemy in enemyList)
        {
            enemy.RemoveLuredArea(targetAreaType);
        }
    }

    // 플레이어 근처에 있는지 확인
    public bool IsNearByPlayer(EnemyType enemyType)
    {
        var targetEnemy = enemyList.Find(e => e.EnemyData.EnemyType == enemyType);
        if (targetEnemy == null)
        {
            Debug.LogError($"Enemy {enemyType} not found");
            return false;
        }

        var nearAreaList = AreaManager.MovableAreaDictionary[targetEnemy.CurrentArea.AreaType];
        var playerArea = AreaManager.Instance.PlayerCurrentArea;

        if (nearAreaList.Contains(playerArea.AreaType))
            return true;
        else
            return false;
    }

    public void PlayKillAnimation(EnemyType enemyType, Action onComplete)
    {
        deathParentObject.SetActive(true);
        deathAnimator.gameObject.SetActive(true);
        deathAnimator.SetTrigger(enemyType.ToString());
        StartCoroutine(HideKillAnimationCoroutine(3f, onComplete));
    }

    private IEnumerator HideKillAnimationCoroutine(float delay, Action onComplete)
    {
        yield return new WaitForSeconds(delay);
        deathParentObject.SetActive(false);
        deathAnimator.gameObject.SetActive(false);
        onComplete?.Invoke();
    }

    void OnDestroy()
    {
        if(TimeManager.Instance != null)
        {
            TimeManager.Instance.OnDayNightChanged -= OnDayNightChanged;
        }
    }
}