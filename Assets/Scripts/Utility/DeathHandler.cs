using System.Collections;
using UnityEngine;

public class DeathHandler : MonoBehaviour
{
    [Header("References")]
    public DeathScreen deathScreen;

    [Header("Nastavení")]
    public float deathDelay = 0.5f;
    public bool freezeTime = true;

    public void OnDie()
    {
        StartCoroutine(DieSequence());
    }

    IEnumerator DieSequence()
    {
        GetComponent<PlayerMovement>().enabled = false;
        GetComponent<PlayerAttack>().enabled = false;

        yield return new WaitForSeconds(deathDelay);

        if (freezeTime)
            Time.timeScale = 0f;

        deathScreen?.Show();
    }
}
