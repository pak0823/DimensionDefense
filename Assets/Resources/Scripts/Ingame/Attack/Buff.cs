using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Buff : MonoBehaviour
{
    public GameObject definitionPrefab { get; private set; }  // Ű�� ����� ���� ������
    int buffCoefficient;
    bool isPlayerShot;

    public void Initialize(int Coefficient, bool isPlayer, float lifeTime, GameObject prefab)
    {
        buffCoefficient = Coefficient;
        isPlayerShot = isPlayer;
        definitionPrefab = prefab;
        Invoke(nameof(ReturnToPool), lifeTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // �÷��̾ ���ٸ�, �� �±׿��� ����
        if (!isPlayerShot && other.CompareTag("Enemy"))
        {
            var dmgable = other.GetComponentInParent<IDamageable>();
            dmgable?.TakeBuff(buffCoefficient);
        }
        // ���� ���ٸ�, �÷��̾� �±׿��� ����
        else if (isPlayerShot && other.CompareTag("Player"))
        {
            var dmgable = other.GetComponentInParent<IDamageable>();
            dmgable?.TakeBuff(buffCoefficient);
        }
    }

    void ReturnToPool()
    {
        CancelInvoke();
        Shared.PoolManager.ReturnProjectile(gameObject);
    }
}
