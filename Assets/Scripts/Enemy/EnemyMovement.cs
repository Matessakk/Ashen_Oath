using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public Transform player;

    protected Rigidbody2D rb;
    protected KnockbackReceiver knockback;

    public float speed = 2f;
    public float detectionRange = 5f;
    public float wanderSpeed = 1f;
    public float wanderTimeMin = 1f;
    public float wanderTimeMax = 3f;

    protected bool playerDetected;
    protected Vector2 wanderDirection;
    float wanderTimer;

    protected bool facingRight = false;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        knockback = GetComponent<KnockbackReceiver>();
        ResetWander();
    }

    protected virtual void Update()
    {
        if (knockback != null && knockback.IsKnocked) return;
        if (player == null) return;

        float dist = Vector2.Distance(transform.position, player.position);
        playerDetected = dist <= detectionRange;

        if (playerDetected)
            ChasePlayer();
        else
            Wander();
    }

    protected virtual void Wander()
    {
        wanderTimer -= Time.deltaTime;

        if (wanderTimer <= 0)
            ResetWander();

        rb.MovePosition(rb.position + wanderDirection * wanderSpeed * Time.deltaTime);
    }

    protected virtual void ChasePlayer()
    {
        Vector2 dir = (player.position - transform.position).normalized;
        FlipByDirection(dir.x);
        rb.MovePosition(rb.position + dir * speed * Time.deltaTime);
    }

    protected void ResetWander()
    {
        float angle = Random.Range(0f, 360f);
        wanderDirection = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)).normalized;
        wanderTimer = Random.Range(wanderTimeMin, wanderTimeMax);
    }

    protected void FlipByDirection(float dirX)
    {
        if (dirX > 0 && !facingRight)
            Flip();
        else if (dirX < 0 && facingRight)
            Flip();
    }

    protected virtual void Flip()
    {
        facingRight = !facingRight;
        Vector3 s = transform.localScale;
        s.x *= -1;
        transform.localScale = s;
    }
}
