using UnityEngine;

public class BatEnemy : EnemyMovement
{
    protected override void Awake()
    {
        base.Awake();
        detectionRange = 7f;
        wanderSpeed = 2f;
    }
}