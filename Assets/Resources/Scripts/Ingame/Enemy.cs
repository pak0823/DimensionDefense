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

    // ��: HP�� 0�� �Ǹ� Disable
    protected override void Die()
    {
        //��� ó�� (Ǯ ��ȯ ��)
        gameObject.SetActive(false);
    }
}

