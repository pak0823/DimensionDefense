using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Enemy.cs
public class Enemy : Character
{
    private EnemyPoolSpawner spawner;

    public override void Initialize()
    {
        base.Initialize();
        spawner = FindObjectOfType<EnemyPoolSpawner>();
    }

    void OnDisable()
    {
        // Ǯ�� ��ȯ
        if (spawner != null)
            spawner.Despawn(this);
    }

    // ��: HP�� 0�� �Ǹ� Disable
    public void Die()
    {
        gameObject.SetActive(false);
    }
}

