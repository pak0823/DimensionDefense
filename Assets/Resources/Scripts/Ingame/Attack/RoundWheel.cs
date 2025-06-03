using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class RoundWheel : MonoBehaviour
{
    int damage = 10;

    void OnTriggerEnter2D(Collider2D other)
    {
        // 플레이어가 쐈다면, 적 태그에만 반응
        if (other.CompareTag("Player"))
        {
            var dmgable = other.GetComponentInParent<IDamageable>();
            dmgable?.TakeDamage(damage);
        }
    }
}
