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

    [Tooltip("프리펩 생명시간 (초)")]
    public float lifeTime = 1f;

    [Tooltip("Animator에서 사용할 트리거 이름")]
    public string attackTrigger = "Attack";

    public static event Action OnHeal;
    public static event Action OnBuff;
    float offsetY = 2.0f;
    string soundName;

    public override void Attack(GameObject self, GameObject target)
    {
        if (buffPrefab == null || target == null) return;

        // 애니메이션 트리거
        var anim = self.GetComponentInChildren<Animator>();
        if (anim != null) anim.SetTrigger(attackTrigger);

        // 누가 쏘는지 구분
        bool isPlayer = self.CompareTag("Player");

        // 풀에서 꺼내기
        var projGO = Shared.PoolManager.SpawnProjectile(
            buffPrefab,
            self.transform.position + Vector3.up * offsetY
        );

        // 프로젝타일 초기화
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
                    Debug.Log("사운드가 없습니다.");
                    break;
            }
        }
    }
}
