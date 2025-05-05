using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Character
{
    private PlayerPoolSpawner spawner;

    public override void Initialize(CharacterStats stats)
    {
        base.Initialize(stats);
        spawner = FindObjectOfType<PlayerPoolSpawner>();
    }

    // ��: HP�� 0�� �Ǹ� Disable
    protected override void Die()
    {
        //��� ó�� (Ǯ ��ȯ ��)
        gameObject.SetActive(false);
    }
}
