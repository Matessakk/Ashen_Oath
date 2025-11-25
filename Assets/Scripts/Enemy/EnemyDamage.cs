using UnityEngine;

public class EnemyDamage : MonoBehaviour
{
    public int damage = 1;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            Debug.Log(name + " hit the player for " + damage + " dmg");
            collision.collider.GetComponent<PlayerHealth>()?.TakeDamage(damage,(collision.transform.position - transform.position).normalized);
        }
    }
}



