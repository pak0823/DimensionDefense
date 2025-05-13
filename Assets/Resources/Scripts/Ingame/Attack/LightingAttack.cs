using System;
using UnityEngine;

public class LightningAttack : AttackStrategySO
{
    [Header("Projectile Settings")]
    [Tooltip("발사할 투사체 Prefab (Rigidbody2D + Projectile 스크립트 포함)")]
    public GameObject projectilePrefab;

    [Tooltip("투사체 생명시간 (초)")]
    public float lifeTime= 1f;

    [Tooltip("Animator에서 사용할 트리거 이름")]
    public string attackTrigger = "Attack";

    public static event Action OnLightiningAttack;

    public override void Attack(GameObject self, GameObject target)
    {
        if (projectilePrefab == null || target == null) return;

        // 애니메이션 트리거
        var anim = self.GetComponentInChildren<Animator>();
        if (anim != null) anim.SetTrigger(attackTrigger);

        // 누가 쏘는지 구분
        bool isPlayer = self.CompareTag("Player");

        Vector3 spawnPos = target.transform.position + Vector3.up * 3f;
        // 풀에서 꺼내기
        var projGO = Shared.PoolManager.SpawnProjectile(
            projectilePrefab,
            spawnPos
        );

        // 프로젝타일 초기화
        var proj = projGO.GetComponent<Lightning>();
        var dmg = self.GetComponent<Character>().attackDamage;

        proj?.Initialize(
            dmg,
            isPlayer,
            projGO.transform,
            projectilePrefab
            );

        //공격 사운드 실행
        //OnLightiningAttack?.Invoke();
    }
}
