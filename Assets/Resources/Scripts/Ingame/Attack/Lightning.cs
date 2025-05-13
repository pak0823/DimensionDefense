using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Lightning : MonoBehaviour
{
    public GameObject definitionPrefab { get; private set; }  // Ű�� ����� ���� ������
    int damage;
    bool isPlayerShot;
    float lifeTime;  // ���ϴ� ����(��)

    public void Initialize(int dmg, bool isPlayer, Transform targetPos ,GameObject prefab)
    {
        Debug.Log("aaa");
        damage = dmg;
        isPlayerShot = isPlayer;
        definitionPrefab = prefab;
        Invoke(nameof(ReturnToPool), lifeTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // �÷��̾ ���ٸ�, �� �±׿��� ����
        if (isPlayerShot && other.CompareTag("Enemy"))
        {
            var dmgable = other.GetComponentInParent<IDamageable>();
            dmgable?.TakeDamage(damage);
            ReturnToPool();
        }
        // ���� ���ٸ�, �÷��̾� �±׿��� ����
        else if (!isPlayerShot && other.CompareTag("Player"))
        {
            var dmgable = other.GetComponentInParent<IDamageable>();
            dmgable?.TakeDamage(damage);
            ReturnToPool();
        }
    }

    void ReturnToPool()
    {
        CancelInvoke();
        Shared.PoolManager.ReturnProjectile(gameObject);
    }
}
