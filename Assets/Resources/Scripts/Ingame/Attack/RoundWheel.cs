using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class RoundWheel : MonoBehaviour
{
    int damage = 10;

    void OnTriggerEnter2D(Collider2D other)
    {
        // �÷��̾ ���ٸ�, �� �±׿��� ����
        if (other.CompareTag("Player"))
        {
            var dmgable = other.GetComponentInParent<IDamageable>();
            dmgable?.TakeDamage(damage);
        }
    }
}
