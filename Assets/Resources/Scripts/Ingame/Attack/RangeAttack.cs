using System;
using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "AttackStrategy/RangeAttack")]
public class RangeAttack : AttackStrategySO
{
    [Header("Projectile Settings")]
    [Tooltip("발사할 투사체 Prefab (Rigidbody2D + Projectile 스크립트 포함)")]
    public GameObject projectilePrefab;

    [Tooltip("투사체 생명시간 (초)")]
    public float lifeTime= 1f;

    [Tooltip("Animator에서 사용할 트리거 이름")]
    public string attackTrigger = "Attack";

    public static event Action OnLightiningAttack;
    public static event Action OnBoomAttack;
    float offsetY = 2.0f;
    string soundName;

    public override void Attack(GameObject self, GameObject target)
    {
        if (projectilePrefab == null || target == null) return;

        // 애니메이션 트리거
        var anim = self.GetComponentInChildren<Animator>();
        if (anim != null) anim.SetTrigger(attackTrigger);

        // 누가 쏘는지 구분
        bool isPlayer = self.CompareTag("Player");

        // 풀에서 꺼내기
        var projGO = Shared.PoolManager.SpawnProjectile(
            projectilePrefab,
            target.transform.position + Vector3.up * offsetY
        );

        // 프로젝타일 초기화
        var proj = projGO.GetComponent<Range>();
        var dmg = self.GetComponent<Character>().attackDamage;

        proj?.Initialize(
            dmg,
            isPlayer,
            lifeTime,
            projectilePrefab
            );

        PlaySound();
    }

    private void PlaySound()
    {
        soundName = projectilePrefab.name;

        if (soundName != null)
        {
            switch (soundName)
            {
                case "Lightning":
                    OnLightiningAttack?.Invoke();
                    break;
                case"Boom":
                    OnBoomAttack?.Invoke();
                    break;
                default:
                    Debug.Log("사운드가 없습니다.");
                    break;
            }
        }
    }
}
