using UnityEngine;

public class FlyingEnemy : EnemyMovement
{
    protected override void Awake()
    {
        base.Awake();
        detectionRange = 7f;
        wanderSpeed = 2f;
    }
}