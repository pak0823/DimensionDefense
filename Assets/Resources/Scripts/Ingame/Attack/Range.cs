using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(Collider2D))]
public class Range : MonoBehaviour
{
    public GameObject definitionPrefab { get; private set; }  // Ű�� ����� ���� ������
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
        // �÷��̾ ���ٸ�, �� �±׿��� ����
        if (isPlayerShot && other.CompareTag("Enemy"))
        {
            var dmgable = other.GetComponentInParent<IDamageable>();
            dmgable?.TakeDamage(damage);
        }
        // ���� ���ٸ�, �÷��̾� �±׿��� ����
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
