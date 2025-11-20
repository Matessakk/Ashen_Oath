using System.Collections;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public WeaponChange weaponChange;
    public GameObject sword;
    public GameObject bow;
    public GameObject arrowPrefab;
    public Transform firePoint;
    public float arrowSpeed = 10f;
    public float weaponVisibleTime = 4f;
    public LineRenderer lineRenderer;
    public Animator animator;

    private bool canAttack = true;
    private Vector2 attackDirection = Vector2.zero;

    private void Start()
    {
        sword.SetActive(false);
        bow.SetActive(false);

        if (lineRenderer != null)
        {
            lineRenderer.enabled = false; 
        }
    }

    private void Update()
    {
        if (!canAttack) return;

        attackDirection = Vector2.zero;

        if (Input.GetKey(KeyCode.W)) attackDirection = Vector2.up;
        else if (Input.GetKey(KeyCode.S)) attackDirection = Vector2.down;
        else if (Input.GetKey(KeyCode.A)) attackDirection = Vector2.left;
        else if (Input.GetKey(KeyCode.D)) attackDirection = Vector2.right;

        
        if (lineRenderer != null)
        {
            if (attackDirection != Vector2.zero)
            {
                lineRenderer.enabled = true;
                lineRenderer.SetPosition(0, transform.position);
                lineRenderer.SetPosition(1, transform.position + (Vector3)attackDirection * 2f);
            }
            else
            {
                lineRenderer.enabled = false;
            }
        }

        if (Input.GetMouseButtonDown(0))
        {

            if (weaponChange.currentWeapon == WeaponChange.WeaponType.sword)
            {
                MeleeAttack();
               
                Debug.Log("Hráè sekl");
            }

            else if (weaponChange.currentWeapon == WeaponChange.WeaponType.bow)
            {
                RangedAttack(attackDirection);
                Debug.Log("Hráè vystøelil");
            }
        }
    }

    private void MeleeAttack()
    {
        StopAllCoroutines();
        StartCoroutine(ShowWeapon(sword));
        if (animator != null)
        {
            animator.SetTrigger("sword");
        }
    }

    private void RangedAttack(Vector2 direction)
    {
        StopAllCoroutines();
        StartCoroutine(ShowWeapon(bow));

        if (direction == Vector2.zero)
            direction = transform.localScale.x > 0 ? Vector2.right : Vector2.left;

        GameObject arrow = Instantiate(arrowPrefab, firePoint.position, Quaternion.identity);
        Rigidbody2D rb = arrow.GetComponent<Rigidbody2D>();
        if (rb == null) return;

        rb.linearVelocity = direction.normalized * arrowSpeed;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        arrow.transform.rotation = Quaternion.Euler(0, 0, angle);

        Arrow arrowScript = arrow.GetComponent<Arrow>();
        if (arrowScript == null)
            arrow.AddComponent<Arrow>();
    }

    private IEnumerator ShowWeapon(GameObject weapon)
    {
        weapon.SetActive(true);
        yield return new WaitForSeconds(weaponVisibleTime);
        weapon.SetActive(false);
    }
}

