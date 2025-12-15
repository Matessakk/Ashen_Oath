using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class EnemyMovement : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    protected Rigidbody2D rb;

    [Header("Stats")]
    public float speed = 2f;
    public float detectionRange = 5f;
    public float wanderSpeed = 1f;
    public float wanderTimeMin = 1f;
    public float wanderTimeMax = 3f;

    protected bool playerDetected = false;
    protected Vector2 wanderDirection;
    private float wanderTimer;

    protected KnockbackReceiver knockback;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        ResetWander();
        knockback = GetComponent<KnockbackReceiver>();
    }

    protected virtual void Update()
    {
        if (knockback.IsKnocked) return;

        float dist = Vector2.Distance(player.position, transform.position);
        playerDetected = dist <= detectionRange;

        if (playerDetected)
        {
            ChasePlayer();
        }
        else
        {
            Wander();
        }
    }

    protected virtual void Wander()
    {
        wanderTimer -= Time.deltaTime;

        if (wanderTimer <= 0)
            ResetWander();

        rb.MovePosition(rb.position + wanderDirection * wanderSpeed * Time.deltaTime);
    }

    protected void ResetWander()
    {
        float angle = Random.Range(0f, 360f);
        wanderDirection = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)).normalized;

        wanderTimer = Random.Range(wanderTimeMin, wanderTimeMax);
    }

    protected virtual void ChasePlayer()
    {
        Vector2 dir = (player.position - transform.position).normalized;
        rb.MovePosition(rb.position + dir * speed * Time.deltaTime);
    }

    protected void MoveTowardsTarget()
    {
        Vector3 dir = (player.position - transform.position).normalized;
        rb.MovePosition(transform.position + dir * speed * Time.deltaTime);
    }

    protected void StopMoving()
    {
        rb.linearVelocity = Vector3.zero;
    }

   


}

