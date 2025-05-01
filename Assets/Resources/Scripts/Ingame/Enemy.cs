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
        // 풀에 반환
        if (spawner != null)
            spawner.Despawn(this);
    }

    // 예: HP가 0이 되면 Disable
    public void Die()
    {
        gameObject.SetActive(false);
    }
}

