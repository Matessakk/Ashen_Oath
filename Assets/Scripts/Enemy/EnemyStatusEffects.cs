using UnityEngine;
using System.Collections;

public class EnemyStatusEffects : MonoBehaviour
{
    public int fireDamagePerTick = 1;
    public float fireTickRate = 0.5f;
    public float fireDuration = 3f;

    public float slowMultiplier = 0.5f;
    public float slowDuration = 2f;

    public float stunDuration = 1.2f;

    public float airKnockbackMultiplier = 1.5f;

    EnemyMovement movement;
    EnemyHealth health;
    Rigidbody2D rb;

    Coroutine fireRoutine;
    Coroutine slowRoutine;
    Coroutine stunRoutine;

    void Awake()
    {
        movement = GetComponent<EnemyMovement>();
        health = GetComponent<EnemyHealth>();
        rb = GetComponent<Rigidbody2D>();
    }

    public void ApplyFire()
    {
        if (fireRoutine != null)
            StopCoroutine(fireRoutine);

        fireRoutine = StartCoroutine(FireRoutine());
    }

    IEnumerator FireRoutine()
    {
        float timer = fireDuration;

        while (timer > 0)
        {
            health.TakePureDamage(fireDamagePerTick);
            yield return new WaitForSeconds(fireTickRate);
            timer -= fireTickRate;
            Debug.Log("Enemy is on fire");
        }
    }

    public void ApplyWater()
    {
        if (slowRoutine != null)
            StopCoroutine(slowRoutine);

        slowRoutine = StartCoroutine(SlowRoutine());
    }

    IEnumerator SlowRoutine()
    {
        float originalSpeed = movement.speed;
        movement.speed *= slowMultiplier;

        yield return new WaitForSeconds(slowDuration);

        movement.speed = originalSpeed;
        Debug.Log("enemy is slowed");
    }

    public void ApplyEarth()
    {
        if (stunRoutine != null)
            StopCoroutine(stunRoutine);

        stunRoutine = StartCoroutine(StunRoutine());
    }

    IEnumerator StunRoutine()
    {
        movement.enabled = false;
        rb.linearVelocity = Vector2.zero;

        yield return new WaitForSeconds(stunDuration);

        movement.enabled = true;

        Debug.Log("enemy is stunned");
    }

    public void ApplyAir(Vector2 dir)
    {
        rb.AddForce(dir * airKnockbackMultiplier, ForceMode2D.Impulse);
    }
}


