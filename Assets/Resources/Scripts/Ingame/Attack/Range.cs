using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(Collider2D))]
public class Range : MonoBehaviour
{
    public GameObject definitionPrefab { get; private set; }  // 키로 사용할 원본 프리팹
    int damage;
    bool isPlayerShot;

    public void Initialize(int dmg, bool isPlayer,float lifeTime ,GameObject prefab)
    {
        Debug.Log("LifeTime: " + lifeTime);
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
        }
        // 적이 쐈다면, 플레이어 태그에만 반응
        else if (!isPlayerShot && other.CompareTag("Player"))
        {
            var dmgable = other.GetComponentInParent<IDamageable>();
            dmgable?.TakeDamage(damage);
        }
    }

    void ReturnToPool()
    {
        CancelInvoke();
        Shared.PoolManager.ReturnProjectile(gameObject);
    }
}
