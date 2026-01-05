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

    [Header("Element SFX - Charge/Swing")]
    public AudioClip fireSfx;
    public AudioClip waterSfx;
    public AudioClip earthSfx;
    public AudioClip airSfx;

    AudioSource audioSource;

    WeaponChange weaponChange;
    WeaponChange.WeaponType lastWeapon;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogError("WeaponCharge: Chyba! Chybí komponenta AudioSource na tomto objektu.");
        }

        weaponChange = GetComponent<WeaponChange>();
        lastWeapon = weaponChange.currentWeapon;
        UpdateEffects();
    }

    public void Init(bool charged)
    {
        isCharged = charged;
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
        }

        if (Input.GetKey(KeyCode.E) && !isCharged)
        {
            chargeTimer += Time.deltaTime;
            if (chargeTimer >= chargeTime)
            {
                isCharged = true;
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

    public void PlayElementSfx()
    {
        if (audioSource == null) return;

        AudioClip clip = null;

        switch (currentElement)
        {
            case Element.Fire: clip = fireSfx; break;
            case Element.Water: clip = waterSfx; break;
            case Element.Earth: clip = earthSfx; break;
            case Element.Air: clip = airSfx; break;
        }

        if (clip != null)
            audioSource.PlayOneShot(clip);
    }
}






