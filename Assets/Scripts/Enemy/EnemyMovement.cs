using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    protected Rigidbody2D rb;

    [Header("Stats")]
    public float speed = 2f;
    public float detectionRange = 5f;

    protected bool playerDetected = false;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    protected virtual void Update()
    {
        float dist = Vector2.Distance(player.position, transform.position);
        playerDetected = dist <= detectionRange;

        if (playerDetected)
            ChasePlayer();
        else
            Wander();
    }

    protected virtual void Wander() { }

    protected virtual void ChasePlayer()
    {
        Vector2 dir = (player.position - transform.position).normalized;
        rb.MovePosition(rb.position + dir * speed * Time.deltaTime);
    }
}
