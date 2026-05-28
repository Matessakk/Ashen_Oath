using UnityEngine;


public class BossProjectile : MonoBehaviour
{
    public int damage = 1;
    public float knockbackForce = 3f;
    public float lifetime = 5f;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Vector2 knockDir = ((Vector2)other.transform.position - (Vector2)transform.position).normalized * knockbackForce;
            other.GetComponent<PlayerHealth>()?.TakeDamage(damage, knockDir);
            Destroy(gameObject);
        }

        if (!other.CompareTag("Enemy") && !other.isTrigger)
            Destroy(gameObject);
    }
}
