using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Character
{
    private PlayerPoolSpawner spawner;

    public override void Initialize()
    {
        base.Initialize();
        spawner = FindObjectOfType<PlayerPoolSpawner>();
    }

    void OnDisable()
    {
        // 풀에 반환
        //if (spawner != null)
        //    spawner.Despawn(this);
    }

    // 예: HP가 0이 되면 Disable
    public void Die()
    {
        gameObject.SetActive(false);
    }
}
