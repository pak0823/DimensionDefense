using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Projectile : MonoBehaviour
{
    public GameObject definitionPrefab { get; private set; }  // 키로 사용할 원본 프리팹
    int damage;
    float speed;
    Vector2 direction;
    bool isPlayerShot;

    SpriteRenderer SpriteRenderer;

    /// <summary>
    /// RangedAttack.Initialize() 에서 호출할 초기화 메서드
    /// </summary>
    public void Initialize(int dmg, float spd, Vector2 dir, bool isPlayer, float lifetime, GameObject prefab)
    {
        damage = dmg;
        speed = spd;
        direction = dir.normalized;
        isPlayerShot = isPlayer;
        definitionPrefab = prefab;
        Invoke(nameof(ReturnToPool), lifetime);
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
        // 플레이어가 쐈다면, 적 태그에만 반응
        if (isPlayerShot && other.CompareTag("Enemy"))
        {
            var dmgable = other.GetComponentInParent<IDamageable>();
            dmgable?.TakeDamage(damage);

            if(!this.CompareTag("Through"))
                ReturnToPool();
        }
        // 적이 쐈다면, 플레이어 태그에만 반응
        else if (!isPlayerShot && other.CompareTag("Player"))
        {
            var dmgable = other.GetComponentInParent<IDamageable>();
            dmgable?.TakeDamage(damage);

            if (!this.CompareTag("Through"))
                ReturnToPool();
        }
    }

    void ReturnToPool()
    {
        CancelInvoke();
        Shared.PoolManager.ReturnProjectile(gameObject);
    }
}
