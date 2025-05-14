using System;
using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "AttackStrategy/RangeAttack")]
public class RangeAttack : AttackStrategySO
{
    [Header("Projectile Settings")]
    [Tooltip("�߻��� ����ü Prefab (Rigidbody2D + Projectile ��ũ��Ʈ ����)")]
    public GameObject projectilePrefab;

    [Tooltip("����ü ����ð� (��)")]
    public float lifeTime= 1f;

    [Tooltip("Animator���� ����� Ʈ���� �̸�")]
    public string attackTrigger = "Attack";

    public static event Action OnLightiningAttack;
    public static event Action OnBoomAttack;
    float offsetY = 2.0f;
    string soundName;

    public override void Attack(GameObject self, GameObject target)
    {
        if (projectilePrefab == null || target == null) return;

        // �ִϸ��̼� Ʈ����
        var anim = self.GetComponentInChildren<Animator>();
        if (anim != null) anim.SetTrigger(attackTrigger);

        // ���� ����� ����
        bool isPlayer = self.CompareTag("Player");

        // Ǯ���� ������
        var projGO = Shared.PoolManager.SpawnProjectile(
            projectilePrefab,
            target.transform.position + Vector3.up * offsetY
        );

        // ������Ÿ�� �ʱ�ȭ
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
                    Debug.Log("���尡 �����ϴ�.");
                    break;
            }
        }
    }
}
