using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Projectile : MonoBehaviour
{
    public GameObject definitionPrefab { get; private set; }  // Ű�� ����� ���� ������
    int damage;
    float speed;
    Vector2 direction;
    bool isPlayerShot;
    float lifeTime = 5f;  // ���ϴ� ����(��)

    SpriteRenderer SpriteRenderer;

    /// <summary>
    /// RangedAttack.Initialize() ���� ȣ���� �ʱ�ȭ �޼���
    /// </summary>
    public void Initialize(int dmg, float spd, Vector2 dir, bool isPlayer, GameObject prefab)
    {
        damage = dmg;
        speed = spd;
        direction = dir.normalized;
        isPlayerShot = isPlayer;
        definitionPrefab = prefab;
        Invoke(nameof(ReturnToPool), lifeTime);
        SpriteRenderer = GetComponent<SpriteRenderer>();

        if (isPlayerShot)
            SpriteRenderer.flipX = true;
        else
            SpriteRenderer.flipX = false;

    }

    void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime);
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
