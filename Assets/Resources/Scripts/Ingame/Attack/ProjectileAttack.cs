using System;
using UnityEngine;

[CreateAssetMenu(menuName = "AttackStrategy/ProjectileAttack")]
public class ProjectileAttack : AttackStrategySO
{
    [Header("Projectile Settings")]
    [Tooltip("발사할 투사체 Prefab (Rigidbody2D + Projectile 스크립트 포함)")]
    public GameObject projectilePrefab;

    [Tooltip("투사체 초기 속도")]
    public float projectileSpeed = 1f;

    [Tooltip("투사체 생명시간 (초)")]
    public float lifeTime = 1f;

    [Tooltip("Animator에서 사용할 트리거 이름")]
    public string attackTrigger = "Attack";

    [Tooltip("SoundManager에서 실행할 이벤트")]
    string soundName;
    public static event Action OnFireBoltAttack;
    public static event Action OnArrowAttack;
    public static event Action OnHolyArrowAttack;
    public static event Action OnWindBreathAttack;

    public override void Attack(GameObject self, GameObject target)
    {
        if (projectilePrefab == null || target == null) return;

        // 애니메이션 트리거
        var anim = self.GetComponentInChildren<Animator>();
        if (anim != null) anim.SetTrigger(attackTrigger);

        // 누가 쏘는지 구분
        bool isPlayer = self.CompareTag("Player");
        Vector2 dir = isPlayer ? Vector2.right : Vector2.left;

        // 풀에서 꺼내기
        var projGO = Shared.PoolManager.SpawnProjectile(
            projectilePrefab,
            self.transform.position
        );

        // 프로젝타일 초기화
        var proj = projGO.GetComponent<Projectile>();
        var dmg = self.GetComponent<Character>().attackDamage;
        proj?.Initialize(
            dmg,
            projectileSpeed,
            dir,
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
                case "FireBolt":
                    OnFireBoltAttack?.Invoke();
                    break;
                case "Arrow":
                    OnArrowAttack?.Invoke();
                    break;
                case "HolyArrow":
                        OnHolyArrowAttack?.Invoke();
                    break;
                case "WindBreath":
                    OnWindBreathAttack?.Invoke();
                    break;
                default:
                    Debug.Log("사운드가 없습니다.");
                    break;
            }
        }
    }
}
