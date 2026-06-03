using System.Collections;
using UnityEngine;

public class BossAI : MonoBehaviour
{
    public float moveSpeed = 3f;
    public float detectionRange = 10f;
    public float meleeRange = 1.5f;
    public float rangedRange = 6f;

    public float meleeCooldown = 1.5f;
    public float rangedCooldown = 3f;

    public int meleeDamage = 2;
    public int rangedDamage = 1;
    public int contactDamage = 1;

    public float meleeKnockbackForce = 5f;
    public float rangedKnockbackForce = 3f;
    public float contactCooldown = 1f;

    public GameObject projectilePrefab;
    public Transform projectileSpawn;
    public float projectileSpeed = 7f;

    [HideInInspector] public Transform player;

    Rigidbody2D rb;
    SpriteRenderer sr;
    PlayerHealth playerHealth;

    bool isBusy;
    float meleeTimer;
    float rangedTimer;
    float contactTimer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        FindActivePlayer();
    }

    void Update()
    {
        // Re-verify the player isn't missing if they moved from another room
        if (player == null)
        {
            FindActivePlayer();
            if (player == null) return;
        }

        if (isBusy) return;

        meleeTimer -= Time.deltaTime;
        rangedTimer -= Time.deltaTime;
        contactTimer -= Time.deltaTime;

        float dist = Vector2.Distance(transform.position, player.position);
        sr.flipX = player.position.x < transform.position.x;

        if (dist > detectionRange)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        if (dist <= meleeRange && meleeTimer <= 0)
        {
            StartCoroutine(MeleeAttack());
            return;
        }

        if (dist <= rangedRange && dist > meleeRange && rangedTimer <= 0)
        {
            StartCoroutine(RangedAttack());
            return;
        }

        Vector2 dir = ((Vector2)player.position - (Vector2)transform.position).normalized;
        rb.linearVelocity = dir * moveSpeed;
    }

    void FindActivePlayer()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
            playerHealth = player.GetComponent<PlayerHealth>();
        }
    }

    IEnumerator MeleeAttack()
    {
        isBusy = true;
        rb.linearVelocity = Vector2.zero;
        meleeTimer = meleeCooldown;

        yield return new WaitForSeconds(0.2f);

        if (player != null)
        {
            float dist = Vector2.Distance(transform.position, player.position);
            if (dist <= meleeRange && playerHealth != null)
            {
                Vector2 knockDir = ((Vector2)player.position - (Vector2)transform.position).normalized * meleeKnockbackForce;
                playerHealth.TakeDamage(meleeDamage, knockDir);
            }
        }

        yield return new WaitForSeconds(0.3f);
        isBusy = false;
    }

    IEnumerator RangedAttack()
    {
        isBusy = true;
        rb.linearVelocity = Vector2.zero;
        rangedTimer = rangedCooldown;

        yield return new WaitForSeconds(0.3f);

        if (projectilePrefab != null && projectileSpawn != null && player != null)
        {
            Vector2 dir = ((Vector2)player.position - (Vector2)projectileSpawn.position).normalized;
            GameObject proj = Instantiate(projectilePrefab, projectileSpawn.position, Quaternion.identity);
            proj.GetComponent<Rigidbody2D>()?.AddForce(dir * projectileSpeed, ForceMode2D.Impulse);

            BossProjectile bp = proj.GetComponent<BossProjectile>();
            if (bp != null)
            {
                bp.damage = rangedDamage;
                bp.knockbackForce = rangedKnockbackForce;
            }
        }

        yield return new WaitForSeconds(0.2f);
        isBusy = false;
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player")) return;
        if (contactTimer > 0) return;

        if (playerHealth == null && collision.gameObject != null)
        {
            playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
        }

        Vector2 knockDir = ((Vector2)collision.transform.position - (Vector2)transform.position).normalized * meleeKnockbackForce;
        playerHealth?.TakeDamage(contactDamage, knockDir);
        contactTimer = contactCooldown;
    }

    public void OnElementHit(WeaponCharge.Element element, Vector2 knockDir)
    {
        if (element == WeaponCharge.Element.Earth)
            StartCoroutine(Stun(1.5f));
    }

    IEnumerator Stun(float duration)
    {
        isBusy = true;
        rb.linearVelocity = Vector2.zero;
        yield return new WaitForSeconds(duration);
        isBusy = false;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, meleeRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, rangedRange);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}