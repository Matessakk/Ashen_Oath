using UnityEngine;

public class KnockbackReceiver : MonoBehaviour
{
    public float knockbackForce = 5f;
    public float knockbackDuration = 0.15f;

    private Rigidbody2D rb;
    private bool beingKnocked = false;
    private float knockTime;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void ApplyKnockback(Vector2 direction)
    {
        if (rb == null) return;

        beingKnocked = true;
        knockTime = knockbackDuration;

        rb.linearVelocity = Vector2.zero; // reset pro jistotu
        rb.AddForce(direction.normalized * knockbackForce, ForceMode2D.Impulse);
    }

    private void Update()
    {
        if (beingKnocked)
        {
            knockTime -= Time.deltaTime;
            if (knockTime <= 0)
            {
                beingKnocked = false;
            }
        }
    }
}

