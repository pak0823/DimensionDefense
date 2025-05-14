using System;
using UnityEngine;

[CreateAssetMenu(menuName = "AttackStrategy/ProjectileAttack")]
public class ProjectileAttack : AttackStrategySO
{
    [Header("Projectile Settings")]
    [Tooltip("�߻��� ����ü Prefab (Rigidbody2D + Projectile ��ũ��Ʈ ����)")]
    public GameObject projectilePrefab;

    [Tooltip("����ü �ʱ� �ӵ�")]
    public float projectileSpeed = 1f;

    [Tooltip("����ü ����ð� (��)")]
    public float lifeTime = 1f;

    [Tooltip("Animator���� ����� Ʈ���� �̸�")]
    public string attackTrigger = "Attack";

    string soundName;
    public static event Action OnFireBoltAttack;
    public static event Action OnArrowAttack;

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
                default:
                    Debug.Log("���尡 �����ϴ�.");
                    break;
            }
        }
    }
}
