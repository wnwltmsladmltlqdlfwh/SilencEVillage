using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public enum EnemyType
{
    Jangsanbeom,
    Seonhju,
    Jibakryeong,
    Agwi,
    EnemyMaxCount,
}

[CreateAssetMenu(fileName = "EnemyDataSO", menuName = "EnemyDataSO", order = 0)]
// Enemy 오브젝트 데이터를 관리하는 ScriptableObject
public class EnemyDataSO : ScriptableObject
{
    // 적 데이터
    public EnemyType EnemyType;
    private string enemyName;
    public string EnemyName => enemyName;
    public Sprite EnemyImage;
    public float MoveDelay;
    public bool isChasePlayer;

    public EnemyDataSO Clone()
    {
        // EnemyDataSO 클론 생성
        EnemyDataSO clone = CreateInstance<EnemyDataSO>();

        // 기본 데이터 복사
        clone.EnemyType = this.EnemyType;
        clone.enemyName = this.EnemyType.ToString();
        clone.EnemyImage = this.EnemyImage;
        clone.MoveDelay = this.MoveDelay;
        clone.isChasePlayer = this.isChasePlayer;

        return clone;
    }
}