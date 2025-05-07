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

    public override void Attack(GameObject self, GameObject target)
    {
        if (projectilePrefab == null || target == null) return;

        // �ִϸ��̼� Ʈ����
        var anim = self.GetComponentInChildren<Animator>();
        if (anim != null) anim.SetTrigger(attackTrigger);

        // 1) ����ü �ν��Ͻ�ȭ
        var projGO = Instantiate(
            projectilePrefab,
            self.transform.position,
            Quaternion.identity
        );

        // 3) Transform �̵� ��� �ʱ�ȭ
        var proj = projGO.GetComponent<Projectile>();
        if (proj != null)
            proj.Initialize(
                self.GetComponent<Character>().attackDamage,
                projectileSpeed        // ������ ���ο��� Vector2.left ����
            );

        // 4) ���� �� �ı�
        Destroy(projGO, lifeTime);
    }
}
