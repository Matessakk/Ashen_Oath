using UnityEngine;

public class BouncerEnemy : EnemyMovement
{
    private Vector2 wanderDir;
    private float wanderTimer;

    public float wanderInterval = 2f;

    protected override void Wander()
    {
        wanderTimer -= Time.deltaTime;

        if (wanderTimer <= 0)
        {
            wanderDir = Random.insideUnitCircle.normalized;
            wanderTimer = wanderInterval;
        }

        rb.MovePosition(rb.position + wanderDir * speed * Time.deltaTime);
    }
}
