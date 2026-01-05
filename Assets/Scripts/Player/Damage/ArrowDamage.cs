using UnityEngine;

public class ArrowDamage : MonoBehaviour
{
    public int baseDamage = 1;
    public int chargedDamage = 5;

    [Header("VFX")]
    public GameObject fireEffect;
    public GameObject waterEffect;
    public GameObject earthEffect;
    public GameObject airEffect;

    [Header("SFX - Fly/Impact")]
    public AudioClip fireSfx;
    public AudioClip waterSfx;
    public AudioClip earthSfx;
    public AudioClip airSfx;

    WeaponCharge.Element element;
    bool charged;
    AudioSource audioSource;

    public void Init(bool isCharged, WeaponCharge.Element el)
    {
        charged = isCharged;
        element = el;

        audioSource = GetComponent<AudioSource>();

        if (!charged) return;

        switch (element)
        {
            case WeaponCharge.Element.Fire:
                if (fireEffect != null) fireEffect.SetActive(true);
                PlaySfx(fireSfx);
                break;

            case WeaponCharge.Element.Water:
                if (waterEffect != null) waterEffect.SetActive(true);
                PlaySfx(waterSfx);
                break;

            case WeaponCharge.Element.Earth:
                if (earthEffect != null) earthEffect.SetActive(true);
                PlaySfx(earthSfx);
                break;

            case WeaponCharge.Element.Air:
                if (airEffect != null) airEffect.SetActive(true);
                PlaySfx(airSfx);
                break;
        }

        Debug.Log("Arrow charged with " + element);
    }

    void PlaySfx(AudioClip clip)
    {
        if (audioSource != null && clip != null)
            audioSource.PlayOneShot(clip);
    }

    void PlaySfxAtLocation(AudioClip clip)
    {
        if (clip != null)
        {
            AudioSource.PlayClipAtPoint(clip, transform.position);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Enemy")) return;

        EnemyHealth eh = collision.GetComponent<EnemyHealth>();
        if (eh == null) return;

        Vector2 knockDir = (collision.transform.position - transform.position).normalized;
        int dmg = charged ? chargedDamage : baseDamage;

        eh.TakeDamage(dmg, knockDir, element);

        if (charged)
        {
            switch (element)
            {
                case WeaponCharge.Element.Fire: PlaySfxAtLocation(fireSfx); break;
                case WeaponCharge.Element.Water: PlaySfxAtLocation(waterSfx); break;
                case WeaponCharge.Element.Earth: PlaySfxAtLocation(earthSfx); break;
                case WeaponCharge.Element.Air: PlaySfxAtLocation(airSfx); break;
            }
        }

        Destroy(gameObject);
    }
}




