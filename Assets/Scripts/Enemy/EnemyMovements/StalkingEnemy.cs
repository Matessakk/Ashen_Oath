using UnityEngine;

public class StalkingEnemy : EnemyMovement
{
    public float lookThreshold = 0.7f;
    public float floatSpeed = 2f;

    protected override void Awake()
    {
        base.Awake();
        rb.gravityScale = 0f;
        rb.linearVelocity = Vector2.zero;
    }

    protected override void Update()
    {
        if (knockback != null && knockback.IsKnocked) return;

        Vector2 toEnemy = (transform.position - player.position).normalized;
        Vector2 playerLookDir = new Vector2(Mathf.Sign(player.localScale.x), 0f);

        float dot = Vector2.Dot(playerLookDir, toEnemy);

        if (dot < lookThreshold)
        {
            Vector2 dir = (player.position - transform.position).normalized;
            rb.linearVelocity = dir * floatSpeed;
            FlipByDirection(dir.x);
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
        }
    }

    protected override void Wander() { }
    protected override void ChasePlayer() { }
}