using UnityEngine;

public class GhostEnemy : EnemyMovement
{
    public float maxDistanceToMove = 10f;

    Transform target;

    void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;
    }

    protected override void Update()
    {
        if (target == null) return;

        float dist = Vector3.Distance(transform.position, target.position);

        if (dist > maxDistanceToMove) return;

        Vector3 directionToGhost = (transform.position - target.position).normalized;
        float dot = Vector3.Dot(target.forward, directionToGhost);

        bool playerIsLookingAtGhost = dot > 0.3f;

        if (!playerIsLookingAtGhost)
            MoveTowardsTarget();
        else
            StopMoving();

        
    }

}


