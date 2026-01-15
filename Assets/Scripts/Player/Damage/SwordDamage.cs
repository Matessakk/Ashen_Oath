using UnityEngine;

public class SwordDamage : MonoBehaviour
{
    public int baseDamage = 1;
    public int chargedDamage = 5;
    public float dmgCd = 0.8f;

    public WeaponCharge weaponCharge;

    [Header("SFX - Impact")]
    public AudioClip fireSfx;
    public AudioClip waterSfx;
    public AudioClip earthSfx;
    public AudioClip airSfx;

    AudioSource audioSource;
    float dmgWait;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Enemy")) return;
        if (Time.time < dmgWait) return;

        bool charged = weaponCharge.TakeCharge();
        int dmg = charged ? chargedDamage : baseDamage;

        Vector2 dir = (collision.transform.position - transform.position).normalized;

        EnemyHealth eh = collision.GetComponent<EnemyHealth>();
        if (eh != null)
            eh.TakeDamage(dmg, dir, weaponCharge.currentElement, charged);

        dmgWait = Time.time + dmgCd;

        if (charged)
            PlayElementSfx(weaponCharge.currentElement);
    }

    void PlayElementSfx(WeaponCharge.Element element)
    {
        if (!audioSource) return;

        AudioClip clip = null;

        switch (element)
        {
            case WeaponCharge.Element.Fire: clip = fireSfx; break;
            case WeaponCharge.Element.Water: clip = waterSfx; break;
            case WeaponCharge.Element.Earth: clip = earthSfx; break;
            case WeaponCharge.Element.Air: clip = airSfx; break;
        }

        if (clip)
            audioSource.PlayOneShot(clip);
    }
}
