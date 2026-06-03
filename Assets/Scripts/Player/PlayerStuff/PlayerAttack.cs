using System.Collections;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("Weapons")]
    public WeaponChange weaponChange;
    public WeaponCharge weaponCharge;
    public GameObject sword;
    public GameObject bow;

    [Header("Sword")]
    public int baseDamage = 1;
    public int chargedDamage = 5;
    public float swordVisibleTime = 0.3f;
    public float hitboxActiveTime = 0.15f;

    [Header("Knockback sily")]
    public float sideKnockback = 8f;
    public float upKnockback = 6f;
    public float downKnockback = 6f;

    [Header("Pogo")]
    public float pogoBounce = 14f;  // sila odrazu hrace nahoru
    public float pogoDownForce = 10f;  // sila knockbacku enemy dolu

    [Header("Hitbox velikost a offset")]
    public Vector2 sideHitboxSize = new Vector2(2f, 1.5f);
    public Vector2 sideHitboxOffset = new Vector2(1.2f, 0f);
    public Vector2 upHitboxSize = new Vector2(1.5f, 2f);
    public Vector2 upHitboxOffset = new Vector2(0f, 1.2f);
    public Vector2 downHitboxSize = new Vector2(1.5f, 2f);
    public Vector2 downHitboxOffset = new Vector2(0f, -0.8f);

    [Header("Arrow")]
    public GameObject arrowPrefab;
    public Transform firePoint;
    public float arrowSpeed = 10f;
    public float arrowCooldown = 0.5f;

    float arrowTimer;
    bool isAttacking;
    bool isFacingRight => transform.localScale.x > 0;

    Rigidbody2D rb;

    GameObject hitboxRight;
    GameObject hitboxLeft;
    GameObject hitboxUp;
    GameObject hitboxDown;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        sword.SetActive(false);
        bow.SetActive(false);

        hitboxRight = CreateHitbox("HitboxRight");
        hitboxLeft = CreateHitbox("HitboxLeft");
        hitboxUp = CreateHitbox("HitboxUp");
        hitboxDown = CreateHitbox("HitboxDown");
    }

    GameObject CreateHitbox(string name)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(transform);
        go.transform.localPosition = Vector3.zero;
        BoxCollider2D col = go.AddComponent<BoxCollider2D>();
        col.isTrigger = true;
        go.SetActive(false);
        return go;
    }

    void Update()
    {
        arrowTimer -= Time.deltaTime;

        if (Input.GetMouseButtonDown(0))
        {
            if (weaponChange.currentWeapon == WeaponChange.WeaponType.sword && !isAttacking)
                MeleeAttack();
            else if (weaponChange.currentWeapon == WeaponChange.WeaponType.bow && arrowTimer <= 0)
            {
                ShootArrow();
                arrowTimer = arrowCooldown;
            }
        }
    }

    void MeleeAttack()
    {
        if (weaponCharge.isCharged)
            weaponCharge.PlayElementSfx();

        bool up = Input.GetKey(KeyCode.W);
        bool down = Input.GetKey(KeyCode.S);

        GameObject hitbox;
        Vector2 enemyKnockDir;
        Vector2 size;
        Vector2 offset;
        bool isPogo = false;

        if (up)
        {
            hitbox = hitboxUp;
            enemyKnockDir = Vector2.up * upKnockback;
            size = upHitboxSize;
            offset = upHitboxOffset;
        }
        else if (down)
        {
            hitbox = hitboxDown;
            enemyKnockDir = Vector2.down * pogoDownForce;
            size = downHitboxSize;
            offset = downHitboxOffset;
            isPogo = true;
        }
        else
        {
            float dir = isFacingRight ? 1f : -1f;
            hitbox = isFacingRight ? hitboxRight : hitboxLeft;
            enemyKnockDir = new Vector2(dir * sideKnockback, 2f);  // lehky vertical pro lepsi feel
            size = sideHitboxSize;
            offset = new Vector2(sideHitboxOffset.x, sideHitboxOffset.y);
        }

        hitbox.GetComponent<BoxCollider2D>().size = size;
        hitbox.transform.localPosition = offset;

        StartCoroutine(DoMeleeAttack(hitbox, enemyKnockDir, isPogo));
    }

    IEnumerator DoMeleeAttack(GameObject hitbox, Vector2 enemyKnockDir, bool isPogo)
    {
        isAttacking = true;
        sword.SetActive(true);
        hitbox.SetActive(true);

        bool charged = weaponCharge.TakeCharge();
        int dmg = charged ? chargedDamage : baseDamage;

        Collider2D[] hits = Physics2D.OverlapBoxAll(
            hitbox.transform.position,
            hitbox.GetComponent<BoxCollider2D>().size,
            0f
        );

        bool hitSomething = false;

        foreach (var hit in hits)
        {
            // Fix: Allows either "Enemy" OR your custom "Boss" tag to pass into the filter!
            if (!hit.CompareTag("Enemy") && !hit.CompareTag("Boss")) continue;

            hitSomething = true;

            EnemyHealth eh = hit.GetComponent<EnemyHealth>();
            if (eh != null)
            {
                eh.TakeDamage(dmg, enemyKnockDir, weaponCharge.currentElement, charged);
            }
            else
            {
                BossHealth bh = hit.GetComponent<BossHealth>();
                bh?.TakeDamage(dmg, enemyKnockDir, weaponCharge.currentElement, charged);
            }
        }

        // pogo bounce — odraz nahoru pri down utoku ktery zasahl nepritele nebo bosse
        if (isPogo && hitSomething)
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, pogoBounce);

        // side/up utok ktery zasahl — maly recoil dozadu pro lepsi feel
        if (!isPogo && hitSomething)
        {
            float recoilDir = isFacingRight ? -1f : 1f;
            rb.linearVelocity = new Vector2(recoilDir * 3f, rb.linearVelocity.y);
        }

        yield return new WaitForSeconds(hitboxActiveTime);
        hitbox.SetActive(false);

        yield return new WaitForSeconds(swordVisibleTime - hitboxActiveTime);
        sword.SetActive(false);
        isAttacking = false;
    }

    void ShootArrow()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0f;
        Vector2 dir = (mousePos - firePoint.position).normalized;

        if (isFacingRight && dir.x < 0) return;
        if (!isFacingRight && dir.x > 0) return;

        GameObject arrow = Instantiate(arrowPrefab, firePoint.position, Quaternion.identity);
        bool charged = weaponCharge.TakeCharge();
        ArrowDamage ad = arrow.GetComponent<ArrowDamage>();
        if (ad != null)
            ad.Init(charged, weaponCharge.currentElement);

        Rigidbody2D arrowRb = arrow.GetComponent<Rigidbody2D>();
        arrowRb.linearVelocity = dir * arrowSpeed;

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        arrow.transform.rotation = Quaternion.Euler(0, 0, angle - 90f);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if (hitboxRight != null && hitboxRight.activeSelf)
            Gizmos.DrawWireCube(hitboxRight.transform.position, sideHitboxSize);
        if (hitboxLeft != null && hitboxLeft.activeSelf)
            Gizmos.DrawWireCube(hitboxLeft.transform.position, sideHitboxSize);
        if (hitboxUp != null && hitboxUp.activeSelf)
            Gizmos.DrawWireCube(hitboxUp.transform.position, upHitboxSize);
        if (hitboxDown != null && hitboxDown.activeSelf)
            Gizmos.DrawWireCube(hitboxDown.transform.position, downHitboxSize);
    }
}