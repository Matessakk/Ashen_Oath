using UnityEngine;

public class WeaponCharge : MonoBehaviour
{
    public enum Element { Fire, Water, Air, Earth }
    public Element currentElement = Element.Fire;

    public float chargeTime = 2f;
    public bool isCharged;

    float chargeTimer;

    [Header("Elements")]
    public GameObject fireEffect;
    public GameObject waterEffect;
    public GameObject airEffect;
    public GameObject earthEffect;
    

    public void Init(bool charged)
    {
        isCharged = charged;

        if (isCharged)
            Debug.Log("Arrow spawned CHARGED");
    
    }


    WeaponChange weaponChange;
    WeaponChange.WeaponType lastWeapon;

    void Awake()
    {
        weaponChange = GetComponent<WeaponChange>();
        lastWeapon = weaponChange.currentWeapon;
        UpdateEffects();
    }

    void Update()
    {
        if (weaponChange.currentWeapon != lastWeapon)
        {
            ResetCharge();
            lastWeapon = weaponChange.currentWeapon;
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            currentElement++;
            if ((int)currentElement > 3)
                currentElement = 0;

            ResetCharge();
            Debug.Log("Switched element: " + currentElement);
        }

        if (Input.GetKey(KeyCode.E) && !isCharged)
        {
            chargeTimer += Time.deltaTime;
            if (chargeTimer >= chargeTime)
            {
                isCharged = true;
                Debug.Log("Weapon charged");
                UpdateEffects();
            }
        }

        

    }

    public bool TakeCharge()
    {
        if (!isCharged) return false;

        isCharged = false;
        chargeTimer = 0;
        UpdateEffects();
        return true;
    }

    void ResetCharge()
    {
        isCharged = false;
        chargeTimer = 0;
        UpdateEffects();
    }

    void UpdateEffects()
    {
        if (fireEffect)
            fireEffect.SetActive(isCharged && currentElement == Element.Fire);

        if (waterEffect)
            waterEffect.SetActive(isCharged && currentElement == Element.Water);

        if (earthEffect)
            earthEffect.SetActive(isCharged && currentElement == Element.Earth);

        if (airEffect)
            airEffect.SetActive(isCharged && currentElement == Element.Air);
    }

}






