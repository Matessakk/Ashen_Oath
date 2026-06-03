using System.Collections;
using UnityEngine;

public class DeathHandler : MonoBehaviour
{
    [Header("Nastavení")]
    public float deathDelay = 0.5f;
    public bool freezeTime = true;

    public void OnDie()
    {
        StartCoroutine(DieSequence());
    }

    IEnumerator DieSequence()
    {
        if (TryGetComponent<PlayerMovement>(out PlayerMovement movement))
        {
            movement.enabled = false;
        }

        if (TryGetComponent<PlayerAttack>(out PlayerAttack attack))
        {
            attack.enabled = false;
        }

        yield return new WaitForSeconds(deathDelay);

        if (freezeTime)
            Time.timeScale = 0f;

        // Fetch the death screen safely through our global UI Manager instance
        if (UIManager.Instance != null && UIManager.Instance.deathScreen != null)
        {
            UIManager.Instance.deathScreen.Show();
        }
        else
        {
            Debug.LogError("DeathHandler: Cannot show death screen because UIManager or DeathScreen reference is missing!");
        }
    }
}