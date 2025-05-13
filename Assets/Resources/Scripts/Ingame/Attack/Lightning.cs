using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Lightning : MonoBehaviour
{
    public GameObject definitionPrefab { get; private set; }  // 키로 사용할 원본 프리팹
    int damage;
    bool isPlayerShot;
    float lifeTime;  // 원하는 수명(초)

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
        // 플레이어가 쐈다면, 적 태그에만 반응
        if (isPlayerShot && other.CompareTag("Enemy"))
        {
            var dmgable = other.GetComponentInParent<IDamageable>();
            dmgable?.TakeDamage(damage);
            ReturnToPool();
        }
        // 적이 쐈다면, 플레이어 태그에만 반응
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
