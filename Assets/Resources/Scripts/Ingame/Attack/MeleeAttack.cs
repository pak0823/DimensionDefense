// BasicAttack.cs
using System;
using UnityEngine;
[CreateAssetMenu(menuName = "AI/BasicAttack")]
public class MeleeAttack :AttackStrategySO
{
    [Tooltip("Animator에서 사용할 트리거 이름")]
    public string attackTrigger = "Attack";

    //public static event Action OnAttack;


    public override void Attack(GameObject self, GameObject target)
    {
        // 1) 애니메이션 실행
        var animator = self.GetComponentInChildren<Animator>();
        if (animator != null)
            animator.SetTrigger(attackTrigger);

        // 2) 공격 스탯 가져오기
        var attacker = self.GetComponent<Character>();
        int damage = attacker != null ? attacker.attackDamage : 0;

        // 3) 대미지 전달
        var characterDamaged = target.GetComponentInParent<Character>();
        var BuildingDamaged = target.GetComponentInParent<IDamageable>();

        if (characterDamaged != null)
            characterDamaged.TakeDamage(damage);
        else if (BuildingDamaged != null)
            BuildingDamaged.TakeDamage(damage);
        else
            Debug.LogWarning($"대상이 없음: characterDamaged = ({characterDamaged}) \n BuildingDamaged = ({BuildingDamaged})");

        //OnAttack?.Invoke();


        //Debug.Log("공격실행: " + self.name + "맞은 적: " + target.name + "대미지: " + damage);
    }
}
