using UnityEngine;

public class KnockbackReceiver : MonoBehaviour
{
    [SerializeField] 
    private float knockbackTime = 0.15f;

    private Rigidbody2D rb;
    private float timer;
    public bool IsKnocked { get; private set; }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void ApplyKnockback(Vector2 force)
    {
        IsKnocked = true;
        timer = knockbackTime;
        rb.linearVelocity = force;
    }

    private void Update()
    {
        if (IsKnocked)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
                IsKnocked = false;
        }
    }
}
