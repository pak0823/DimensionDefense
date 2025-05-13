using System;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/RangedAttack")]
public class RangedAttack : AttackStrategySO
{
    [Header("Projectile Settings")]
    [Tooltip("�߻��� ����ü Prefab (Rigidbody2D + Projectile ��ũ��Ʈ ����)")]
    public GameObject projectilePrefab;

    [Tooltip("����ü �ʱ� �ӵ�")]
    public float projectileSpeed = 10f;

    [Tooltip("����ü ����ð� (��)")]
    public float lifeTime = 5f;

    [Tooltip("Animator���� ����� Ʈ���� �̸�")]
    public string attackTrigger = "Attack";

    public static event Action OnFireBoltAttack;

    public override void Attack(GameObject self, GameObject target)
    {
        if (projectilePrefab == null || target == null) return;

        // �ִϸ��̼� Ʈ����
        var anim = self.GetComponentInChildren<Animator>();
        if (anim != null) anim.SetTrigger(attackTrigger);

        // ���� ����� ����
        bool isPlayer = self.CompareTag("Player");
        Vector2 dir = isPlayer ? Vector2.right : Vector2.left;

        // Ǯ���� ������
        var projGO = Shared.PoolManager.SpawnProjectile(
            projectilePrefab,
            self.transform.position
        );

        // ������Ÿ�� �ʱ�ȭ
        var proj = projGO.GetComponent<Projectile>();
        var dmg = self.GetComponent<Character>().attackDamage;
        proj?.Initialize(
            dmg,
            projectileSpeed,
            dir,
            isPlayer,
            projectilePrefab
        );

        //���� ���� ����
        OnFireBoltAttack?.Invoke();
    }
}
