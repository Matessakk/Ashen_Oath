using UnityEngine;

public class sinFloaterEnemy : EnemyMovement
{
    public float floatAmplitude = 0.5f;
    public float floatSpeed = 2f;

    float baseY;
    

    protected override void Awake()
    {
        base.Awake();
        baseY = transform.position.y;
    }

    protected override void Wander()
    {
        ApplyFloating(Vector2.zero);
        FacePlayer();
    }

    protected override void ChasePlayer()
    {
        Vector2 dir = (player.position - transform.position).normalized;
        ApplyFloating(dir * speed);
        FacePlayer();
    }

    void ApplyFloating(Vector2 move)
    {
        float yOffset = Mathf.Sin(Time.time * floatSpeed) * floatAmplitude;
        Vector2 target = rb.position + move * Time.deltaTime;
        target.y = baseY + yOffset;
        rb.MovePosition(target);
    }

    void FacePlayer()
    {
        if (player == null) return;

        if (player.position.x > transform.position.x && !facingRight)
            Flip();
        else if (player.position.x < transform.position.x && facingRight)
            Flip();
    }

    
}
