using UnityEngine;

public class ElementIndicator : MonoBehaviour
{
    public WeaponCharge weaponCharge;
    public SpriteRenderer spriteRenderer;

    // Barvy pro každý element
    public Color fireColor = new Color(1f, 0.3f, 0f);    // oranžová
    public Color waterColor = new Color(0f, 0.5f, 1f);    // modrá
    public Color airColor = new Color(0.8f, 0.9f, 1f);  // svìtle modrá
    public Color earthColor = new Color(0.4f, 0.25f, 0f); // hnìdá

    // Animace pulzování když je charged
    public float pulseSpeed = 3f;
    public float pulseAmount = 0.15f;

    Vector3 baseScale;

    void Start()
    {
        baseScale = transform.localScale;
    }

    void Update()
    {
        // Nastav barvu podle elementu
        Color target = weaponCharge.currentElement switch
        {
            WeaponCharge.Element.Fire => fireColor,
            WeaponCharge.Element.Water => waterColor,
            WeaponCharge.Element.Air => airColor,
            WeaponCharge.Element.Earth => earthColor,
            _ => Color.white
        };

        // Když je charged - plná barva, jinak tmavší/prùhledná
        if (!weaponCharge.isCharged)
            target = new Color(target.r * 0.4f, target.g * 0.4f, target.b * 0.4f, 0.5f);

        spriteRenderer.color = target;

        // Pulzování když charged
        if (weaponCharge.isCharged)
        {
            float pulse = 1f + Mathf.Sin(Time.time * pulseSpeed) * pulseAmount;
            transform.localScale = baseScale * pulse;
        }
        else
        {
            transform.localScale = baseScale;
        }
    }
}
