using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;

public class EnemyManager : Singleton<EnemyManager>
{
    [Header("Enemy Settings")]
    [SerializeField] private float moveInterval = 2f;
    [SerializeField] private float randomMoveChance = 0.3f;

    [Header("Enemy List")]
    [SerializeField] private List<EnemyBase> enemyList = new List<EnemyBase>();

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