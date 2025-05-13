using System;
using UnityEngine;

public class LightningAttack : AttackStrategySO
{
    [Header("Projectile Settings")]
    [Tooltip("�߻��� ����ü Prefab (Rigidbody2D + Projectile ��ũ��Ʈ ����)")]
    public GameObject projectilePrefab;

    [Tooltip("����ü ����ð� (��)")]
    public float lifeTime= 1f;

    [Tooltip("Animator���� ����� Ʈ���� �̸�")]
    public string attackTrigger = "Attack";

    public static event Action OnLightiningAttack;

    public override void Attack(GameObject self, GameObject target)
    {
        if (projectilePrefab == null || target == null) return;

        // �ִϸ��̼� Ʈ����
        var anim = self.GetComponentInChildren<Animator>();
        if (anim != null) anim.SetTrigger(attackTrigger);

        // ���� ����� ����
        bool isPlayer = self.CompareTag("Player");

        Vector3 spawnPos = target.transform.position + Vector3.up * 3f;
        // Ǯ���� ������
        var projGO = Shared.PoolManager.SpawnProjectile(
            projectilePrefab,
            spawnPos
        );

        // ������Ÿ�� �ʱ�ȭ
        var proj = projGO.GetComponent<Lightning>();
        var dmg = self.GetComponent<Character>().attackDamage;

        proj?.Initialize(
            dmg,
            isPlayer,
            projGO.transform,
            projectilePrefab
            );

        //���� ���� ����
        //OnLightiningAttack?.Invoke();
    }
}
