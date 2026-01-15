using UnityEngine;

public class Fireball : MonoBehaviour
{
    public float lifeTime = 3f;
    public int damage = 1;
    public LayerMask groundLayer;

    Rigidbody2D rb;

    [Header("SFX")]
    public AudioClip fireSFX;

    AudioSource audioSource;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        Destroy(gameObject, lifeTime);
    }

    public void Init(Vector2 direction, float speed)
    {
        rb.linearVelocity = direction.normalized * speed;

        audioSource.GetComponent<AudioSource>(); 

        PlaySfx(fireSFX);
    }

    void PlaySfx(AudioClip clip)
    {
        if (audioSource && clip)
            audioSource.PlayOneShot(clip);
    }

    void PlaySfxAtLocation(AudioClip clip)
    {
        if (clip)
            AudioSource.PlayClipAtPoint(clip, transform.position);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerHealth ph = collision.GetComponent<PlayerHealth>();
            Vector2 dir = (collision.transform.position - transform.position).normalized;
            if (ph != null)
                ph.TakeDamage(damage, dir) ;

            Destroy(gameObject);
            return;
        }

        if (((1 << collision.gameObject.layer) & groundLayer) != 0)
        {
            Destroy(gameObject);
        }
    }
}

