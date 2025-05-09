using UnityEngine;

[CreateAssetMenu(menuName = "AI/RangedAttack")]
public class RangedAttack : AttackStrategySO
{
    [Header("Projectile Settings")]
    [Tooltip("발사할 투사체 Prefab (Rigidbody2D + Projectile 스크립트 포함)")]
    public GameObject projectilePrefab;

    [Tooltip("투사체 초기 속도")]
    public float projectileSpeed = 10f;

    [Tooltip("투사체 생명시간 (초)")]
    public float lifeTime = 5f;

    [Tooltip("Animator에서 사용할 트리거 이름")]
    public string attackTrigger = "Attack";

    public override void Attack(GameObject self, GameObject target)
    {
        if (projectilePrefab == null || target == null) return;

        // 애니메이션 트리거
        var anim = self.GetComponentInChildren<Animator>();
        if (anim != null) anim.SetTrigger(attackTrigger);

        // 1) 투사체 인스턴스화
        var projGO = Shared.PoolManager.Spawn(projectilePrefab, self.transform.position);
        var proj = projGO.GetComponent<Projectile>();
        proj.Initialize(self.GetComponent<Character>().attackDamage, projectileSpeed);

        // 2) 수명 후 파괴
        Destroy(projGO, lifeTime);
    }
}
