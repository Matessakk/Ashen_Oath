using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] float speed = 8f;

    [Header("Jump")]
    [SerializeField] float jumpForce = 16f;
    [SerializeField] float jumpCutMultiplier = 0.45f;
    [SerializeField] float fallMultiplier = 2.5f;

    [Header("Dash")]
    [SerializeField] float dashForce = 20f;
    [SerializeField] float dashDuration = 0.2f;
    [SerializeField] float dashCooldown = 1f;

    Rigidbody2D rb;
    KnockbackReceiver knockback;

    bool isFacingRight = true;
    bool isDashing;
    bool canDash = true;
    bool hasAirDashed;
    bool isGrounded;
    bool canJump;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        knockback = GetComponent<KnockbackReceiver>();
    }

    void Update()
    {
        if (isDashing) return;
        if (knockback != null && knockback.IsKnocked) return;

        float h = Input.GetAxisRaw("Horizontal");
        rb.linearVelocity = new Vector2(h * speed, rb.linearVelocity.y);

        if (Input.GetButtonDown("Jump") && canJump)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            canJump = false;
        }

        if (Input.GetButtonUp("Jump") && rb.linearVelocity.y > 0)
        {
            rb.linearVelocity = new Vector2(
                rb.linearVelocity.x,
                rb.linearVelocity.y * jumpCutMultiplier
            );
        }

        if (rb.linearVelocity.y < 0)
        {
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * fallMultiplier * Time.deltaTime;
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (isGrounded && canDash)
            {
                StartCoroutine(Dash());
                StartCoroutine(DashCooldown());
            }
            else if (!isGrounded && !hasAirDashed)
            {
                StartCoroutine(Dash());
                hasAirDashed = true;
            }
        }

        Flip(h);
    }

    IEnumerator Dash()
    {
        isDashing = true;
        float g = rb.gravityScale;
        rb.gravityScale = 0;
        rb.linearVelocity = new Vector2((isFacingRight ? 1 : -1) * dashForce, 0);
        yield return new WaitForSeconds(dashDuration);
        rb.gravityScale = g;
        isDashing = false;
    }

    IEnumerator DashCooldown()
    {
        canDash = false;
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    void Flip(float h)
    {
        if (h > 0 && !isFacingRight || h < 0 && isFacingRight)
        {
            isFacingRight = !isFacingRight;
            Vector3 s = transform.localScale;
            s.x *= -1;
            transform.localScale = s;
        }
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            canJump = true;
            hasAirDashed = false;
        }
    }

    void OnCollisionExit2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }
}