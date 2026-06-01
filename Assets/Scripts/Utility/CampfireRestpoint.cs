using System.Collections;
using UnityEngine;

public class CampfireRestPoint : MonoBehaviour
{
    [Header("Heal")]
    public int healAmount = 999;
    public float healDuration = 1.2f;

    [Header("References")]
    public CampfireUI promptUI;
    public SkillTreeUI skillTreeUI;

    [Header("FX (volitelné)")]
    public ParticleSystem healEffect;
    public AudioClip healSfx;

    AudioSource _audio;
    PlayerHealth _playerInRange;
    bool _isResting;

    void Awake()
    {
        _audio = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (_playerInRange == null || _isResting) return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            SpawnManager.Instance?.SetCampfire(transform.position);
            SaveSystem.Instance?.SaveGame();

            if (_playerInRange.currentHealth >= _playerInRange.maxHealth)
            {
                promptUI?.ShowAlreadyFull();
                return;
            }
            StartCoroutine(DoRest(_playerInRange));
        }
    }

    IEnumerator DoRest(PlayerHealth player)
    {
        Debug.Log("DoRest started");
        _isResting = true;
        promptUI?.HidePrompt();

        int amount = healAmount >= 999
            ? player.maxHealth - player.currentHealth
            : healAmount;

        player.currentHealth += amount;
        player.currentHealth = Mathf.Clamp(player.currentHealth, 0, player.maxHealth);
        player.onHealthChanged?.Invoke(player.currentHealth, player.maxHealth);

        if (healEffect != null) healEffect.Play();
        if (healSfx != null && _audio != null)
            _audio.PlayOneShot(healSfx);

        yield return new WaitForSeconds(healDuration);
        Debug.Log("DoRest after wait");

        _isResting = false;

        if (_playerInRange != null)
            promptUI?.ShowPrompt();

        FindFirstObjectByType<EnemySpawner>()?.RespawnEnemies();

        SpawnManager.Instance?.SetCampfire(transform.position);
        Debug.Log($"SpawnManager instance: {SpawnManager.Instance}");

        Debug.Log($"SaveSystem instance: {SaveSystem.Instance}");
        SaveSystem.Instance?.SaveGame();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            _playerInRange = other.GetComponent<PlayerHealth>();
            if (_playerInRange != null)
            {
                promptUI?.ShowPrompt();
                skillTreeUI?.Show();
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            _playerInRange = null;
            _isResting = false;
            promptUI?.HidePrompt();
            skillTreeUI?.Hide();
        }
    }
}