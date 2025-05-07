using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Projectile : MonoBehaviour
{
    int damage;
    float speed;

    /// <summary>
    /// RangedAttack.Initialize() 에서 호출할 초기화 메서드
    /// </summary>
    public void Initialize(int dmg, float spd)
    {
        damage = dmg;
        speed = spd;
    }

    void Update()
    {
        transform.Translate(Vector2.left * speed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        var building = other.GetComponentInParent<IDamageable>();
        var character = other.GetComponentInParent<Character>();
        
        if (other.CompareTag("Player"))
        {
            //Debug.Log("name: " + other.name + "\n tag: " + other.tag + "\n Damage: " + damage);

            if (building != null)
                building.TakeDamage(damage);

            if (character != null)
                character.TakeDamage(damage);
                Destroy(gameObject);
        } 
    }
}
