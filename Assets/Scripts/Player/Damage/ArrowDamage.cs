using UnityEngine;

public class ArrowDamage : MonoBehaviour
{
    public int baseDamage = 1;
    public int chargedDamage = 5;
    [Header("Elements")]
    public GameObject fireEffect;
    public GameObject waterEffect;
    public GameObject airEffect;
    public GameObject earthEffect;
    

    bool isCharged;

    WeaponCharge.Element element;



    public void Init(bool charged, WeaponCharge.Element currentElement)
    {
        isCharged = charged;
        element = currentElement;

        if (fireEffect)
            fireEffect.SetActive(isCharged && element == WeaponCharge.Element.Fire);

        if (waterEffect)
            waterEffect.SetActive(isCharged && element == WeaponCharge.Element.Water);

        if (earthEffect)
            earthEffect.SetActive(isCharged && currentElement == WeaponCharge.Element.Earth);

        if (airEffect)
            airEffect.SetActive(isCharged && currentElement == WeaponCharge.Element.Air);

        
    }





    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Enemy")) return;

        Vector2 knockDir = (collision.transform.position - transform.position).normalized;

        EnemyHealth eh = collision.GetComponent<EnemyHealth>();
        if (eh != null)
            eh.TakeDamage(isCharged ? chargedDamage : baseDamage, knockDir);

        Destroy(gameObject);
    }
}



