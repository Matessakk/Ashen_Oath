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
    public float swordVisibleTime = 0.3f;

    private bool canAttack = true;

    private void Start()
    {
        sword.SetActive(false);
        bow.SetActive(false);
    }

    private void Update()
    {
        if (!canAttack) return;

        if (Input.GetMouseButtonDown(0))
        {
            if (weaponChange.currentWeapon == WeaponChange.WeaponType.sword)
            {
                MeleeAttack();
            }
            else if (weaponChange.currentWeapon == WeaponChange.WeaponType.bow)
            {
                ShootArrow();
            }
        }
    }

    private void MeleeAttack()
    {
        StopAllCoroutines();
        StartCoroutine(ShowWeapon(sword));
    }

    private IEnumerator ShowWeapon(GameObject weapon)
    {
        weapon.SetActive(true);
        yield return new WaitForSeconds(swordVisibleTime);
        weapon.SetActive(false);
    }

    private void ShootArrow()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0f;

        Vector2 direction = (mousePos - firePoint.position).normalized;

        GameObject arrow = Instantiate(arrowPrefab, firePoint.position, Quaternion.identity);
        Rigidbody2D rb = arrow.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            AudioSource
            rb.linearVelocity = direction * arrowSpeed;

            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            arrow.transform.rotation = Quaternion.Euler(0, 0, angle - 90f);
        }
    }
}


