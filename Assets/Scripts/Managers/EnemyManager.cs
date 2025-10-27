using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;

public class EnemyManager : Singleton<EnemyManager>
{
    [Header("Enemy List")]
    [SerializeField] private List<EnemyBase> enemyList = new List<EnemyBase>();


    public void SetupEnemyList()
    {
        foreach(var enemy in enemyList)
        {
            enemy.InitEnemyData();
            Debug.Log($"Enemy {enemy.EnemyData.EnemyName} initialized");
        }
    }

    public void ActivateSoundLure(AreaType targetAreaType)
    {
        foreach(var enemy in enemyList)
        {
            enemy.AddLuredArea(targetAreaType);
        }
    }

    public void DeactivateSoundLure(AreaType targetAreaType)
    {
        foreach(var enemy in enemyList)
        {
            enemy.RemoveLuredArea(targetAreaType);
        }
    }
}