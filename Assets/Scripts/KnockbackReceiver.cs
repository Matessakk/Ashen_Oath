using UnityEngine;

public class KnockbackReceiver : MonoBehaviour
{
    [SerializeField] float knockbackTime = 0.2f;
    [SerializeField] float knockbackDecay = 8f;  // jak rychle se zpomalí

    Rigidbody2D rb;
    float timer;

    public bool IsKnocked { get; private set; }

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void ApplyKnockback(Vector2 force)
    {
        IsKnocked = true;
        timer = knockbackTime;
        rb.linearVelocity = force;
    }

    void Update()
    {
        if (!IsKnocked) return;

        timer -= Time.deltaTime;

        // plynulé zpomalení knockbacku
        rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, new Vector2(0, rb.linearVelocity.y), knockbackDecay * Time.deltaTime);

        if (timer <= 0)
            IsKnocked = false;
    }
}