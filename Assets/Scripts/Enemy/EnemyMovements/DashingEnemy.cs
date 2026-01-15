using UnityEngine;

public class DashingEnemy : EnemyMovement
{
    public float dashForce = 8f;
    public float dashCooldown = 2f;
    public float dashDuration = 0.2f;
    public float frontAngle = 60f;

    float nextDashTime;
    bool isDashing;
    float dashEndTime;

    protected override void Update()
    {
        if (knockback.IsKnocked) return;

        if (isDashing)
        {
            if (Time.time >= dashEndTime)
                EndDash();
            return;
        }

        base.Update();

        if (!playerDetected) return;

        if (Time.time >= nextDashTime && PlayerIsInFront())
            StartDash();
    }

    bool PlayerIsInFront()
    {
        float dirX = player.position.x - transform.position.x;
        float facingDir = transform.localScale.x > 0 ? 1f : -1f;

        return Mathf.Sign(dirX) == Mathf.Sign(facingDir);
    }

    void StartDash()
    {
        isDashing = true;

        float dirX = Mathf.Sign(player.position.x - transform.position.x);

        FlipByDirection(dirX);

        rb.linearVelocity = new Vector2(dirX * dashForce, 0f);

        dashEndTime = Time.time + dashDuration;
        nextDashTime = Time.time + dashCooldown;
    }

    void EndDash()
    {
        isDashing = false;
        rb.linearVelocity = Vector2.zero;
    }
}




