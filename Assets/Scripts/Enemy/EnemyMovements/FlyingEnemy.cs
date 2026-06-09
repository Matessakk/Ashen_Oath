using UnityEngine;
public class FlyingEnemy : EnemyMovement
{
    protected override void Awake()
    {
        base.Awake();
        rb.gravityScale = 0f;
        detectionRange = 7f;
        wanderSpeed = 2f;
    }

    protected override void Wander()
    {
        wanderTimer -= Time.deltaTime;
        if (wanderTimer <= 0) ResetWander();
        // Full 2D movement, no gravity to preserve
        rb.linearVelocity = wanderDirection * wanderSpeed;
    }

    protected override void ChasePlayer()
    {
        Vector2 dir = (player.position - transform.position).normalized;
        FlipByDirection(dir.x);
        rb.linearVelocity = dir * speed;
    }
}