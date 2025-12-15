using System.Collections;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public WeaponChange weaponChange;
    public WeaponCharge weaponCharge;

    public GameObject sword;
    public GameObject bow;
    public GameObject arrowPrefab;
    public Transform firePoint;
    public float arrowSpeed = 10f;
    public float swordVisibleTime = 0.3f;

    void Start()
    {
        sword.SetActive(false);
        bow.SetActive(false);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (weaponChange.currentWeapon == WeaponChange.WeaponType.sword)
                MeleeAttack();
            else if (weaponChange.currentWeapon == WeaponChange.WeaponType.bow)
                ShootArrow();
        }
    }

    void MeleeAttack()
    {
        StartCoroutine(ShowWeapon(sword));
    }

    IEnumerator ShowWeapon(GameObject weapon)
    {
        weapon.SetActive(true);
        yield return new WaitForSeconds(swordVisibleTime);
        weapon.SetActive(false);
    }

    void ShootArrow()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0f;

        Vector2 dir = (mousePos - firePoint.position).normalized;

        GameObject arrow = Instantiate(arrowPrefab, firePoint.position, Quaternion.identity);

        bool charged = weaponCharge.TakeCharge();
        Debug.Log("Arrow shot | charged = " + charged);

        
        ArrowDamage ad = arrow.GetComponent<ArrowDamage>();
        if (ad != null)
            ad.Init(charged, weaponCharge.currentElement);


        Rigidbody2D rb = arrow.GetComponent<Rigidbody2D>();
        rb.linearVelocity = dir * arrowSpeed;

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        arrow.transform.rotation = Quaternion.Euler(0, 0, angle - 90f);
    }

}





