using UnityEngine;

public class ArrowDamage : MonoBehaviour
{
    public int damage = 1;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            Vector2 knockDir = (collision.transform.position - transform.position).normalized;

            EnemyHealth eh = collision.GetComponent<EnemyHealth>();
            if (eh != null)
            {
                eh.TakeDamage(damage, knockDir);
            }

            Destroy(gameObject);
        }
    }
}

