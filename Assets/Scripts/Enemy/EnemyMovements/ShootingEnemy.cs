using UnityEngine;
using System.Collections;

public class ShootingEnemy : EnemyMovement
{
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float shootCooldown = 1.5f;
    public float projectileSpeed = 6f;

    bool canShoot = true;
    

    protected override void Update()
    {
        if (knockback != null && knockback.IsKnocked) return;
        if (player == null) return;

        float dist = Vector2.Distance(transform.position, player.position);

        if (dist <= detectionRange)
        {
            FacePlayer();

            if (canShoot)
                StartCoroutine(Shoot());
        }
    }

    IEnumerator Shoot()
    {
        canShoot = false;

        GameObject proj = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        Rigidbody2D rbProj = proj.GetComponent<Rigidbody2D>();

        Vector2 dir = (player.position - firePoint.position).normalized;
        rbProj.linearVelocity = dir * projectileSpeed;

        yield return new WaitForSeconds(shootCooldown);
        canShoot = true;
    }

    void FacePlayer()
    {
        if (player.position.x > transform.position.x && !facingRight)
            Flip();
        else if (player.position.x < transform.position.x && facingRight)
            Flip();
    }

    

    protected override void Wander() { }
    protected override void ChasePlayer() { }
}


