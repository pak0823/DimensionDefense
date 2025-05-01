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
        // Ǯ�� ��ȯ
        //if (spawner != null)
        //    spawner.Despawn(this);
    }

    // ��: HP�� 0�� �Ǹ� Disable
    public void Die()
    {
        gameObject.SetActive(false);
    }
}
