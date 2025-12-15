using UnityEngine;

public class SkullEnemy : EnemyMovement
{
    public float dashForce = 8f;
    public float dashCooldown = 2f;
    public float dashDuration = .2f;
    public float frontAngle = 60f;

    float nextDashTime;
    bool isDashing = false;
    float dashEndTime;

    protected override void Update()
    {
        base.Update();

        if (!playerDetected) return;

        if (!isDashing && Time.time >= nextDashTime && PlayerIsInFront())
            StartDash();

        if (isDashing && Time.time >= dashEndTime)
            EndDash();
    }

    bool PlayerIsInFront()
    {
        Vector2 toPlayer = (player.position - transform.position).normalized;
        Vector2 forward = transform.right;

        float angle = Vector2.Angle(forward, toPlayer);

        return angle < frontAngle * 0.5f;
    }

    void StartDash()
    {
        isDashing = true;

        Vector2 dir = (player.position - transform.position).normalized;
        rb.linearVelocity = dir * dashForce;

        dashEndTime = Time.time + dashDuration;
        nextDashTime = Time.time + dashCooldown;
    }

    void EndDash()
    {
        isDashing = false;
        rb.linearVelocity = Vector2.zero;
    }
}


