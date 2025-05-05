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

    // 예: HP가 0이 되면 Disable
    protected override void Die()
    {
        //사망 처리 (풀 반환 등)
        gameObject.SetActive(false);
    }
}
