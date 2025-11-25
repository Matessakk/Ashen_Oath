using UnityEngine;

public class EnemyAudio : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip hitSound;

    public void PlayHitSound()
    {
        audioSource.PlayOneShot(hitSound);
    }
}
