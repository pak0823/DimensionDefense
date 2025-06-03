using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AttackStrategy/BuffAttack")]
public class BuffAttack : AttackStrategySO
{
    [Header("Prefab Settings")]
    public GameObject buffPrefab;

    public CharacterDefinition characterDef;

    [Tooltip("������ ����ð� (��)")]
    public float lifeTime = 1f;

    [Tooltip("Animator���� ����� Ʈ���� �̸�")]
    public string attackTrigger = "Attack";

    public static event Action OnHeal;
    public static event Action OnBuff;
    float offsetY = 2.0f;
    string soundName;

    public override void Attack(GameObject self, GameObject target)
    {
        if (buffPrefab == null || target == null) return;

        // �ִϸ��̼� Ʈ����
        var anim = self.GetComponentInChildren<Animator>();
        if (anim != null) anim.SetTrigger(attackTrigger);

        // ���� ����� ����
        bool isPlayer = self.CompareTag("Player");

        // Ǯ���� ������
        var projGO = Shared.PoolManager.SpawnProjectile(
            buffPrefab,
            self.transform.position + Vector3.up * offsetY
        );

        // ������Ÿ�� �ʱ�ȭ
        var proj = projGO.GetComponent<Buff>();
        var heal = (int)(self.GetComponent<Character>().maxHp * 0.3f);

        proj?.Initialize(
            heal,
            isPlayer,
            lifeTime,
            buffPrefab
            );

        PlaySound();
    }

    private void PlaySound()
    {
        soundName = buffPrefab.name;

        if (soundName != null)
        {
            switch (soundName)
            {
                case "Heal":
                    OnHeal?.Invoke();
                    break;
                case "Buff":
                    OnBuff?.Invoke();
                    break;
                default:
                    Debug.Log("���尡 �����ϴ�.");
                    break;
            }
        }
    }
}
