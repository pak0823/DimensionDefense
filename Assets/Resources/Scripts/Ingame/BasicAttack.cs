// BasicAttack.cs
using UnityEngine;
[CreateAssetMenu(menuName = "AI/BasicAttack")]
public class BasicAttack :AttackStrategySO
{
    [Tooltip("Animator에서 사용할 트리거 이름")]
    public string attackTrigger = "Attack";


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
        var victim = target.GetComponentInParent<Character>();
        if (victim != null)
            victim.TakeDamage(damage);
        else
            Debug.LogWarning($"BasicAttack: 대상에 Character 없음 ({target.name})");

        var dmgable = target.GetComponentInParent<IDamageable>();
        if (dmgable != null)
            dmgable.TakeDamage(damage);

        Debug.Log("공격실행: " + self.name + "맞은 적: " + target.name + "대미지: " + damage);
    }
}
