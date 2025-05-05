using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Enemy.cs
public class Enemy : Character
{
    private EnemyPoolSpawner spawner;

    public override void Initialize(CharacterStats stats)
    {
        base.Initialize(stats);
        spawner = FindObjectOfType<EnemyPoolSpawner>();
    }

    // 예: HP가 0이 되면 Disable
    protected override void Die()
    {
        //사망 처리 (풀 반환 등)
        gameObject.SetActive(false);
    }
}

