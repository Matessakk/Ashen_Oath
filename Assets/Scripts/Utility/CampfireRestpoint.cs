using System.Collections;
using UnityEngine;

public class CampfireRestPoint : MonoBehaviour
{
    [Header("Heal")]
    public int healAmount = 999;
    public float healDuration = 1.2f;

    [Header("FX (Optional)")]
    public ParticleSystem healEffect;
    public AudioClip healSfx;

    AudioSource _audio;
    PlayerHealth _playerInRange;
    bool _isResting;

    private CampfireUI PromptUI => UIManager.Instance != null ? UIManager.Instance.campfireUI : null;
    private SkillTreeUI SkillTreePanel => UIManager.Instance != null ? UIManager.Instance.skillTreeUI : null;

    void Awake()
    {
        _audio = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (_playerInRange == null || _isResting) return;
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (SpawnManager.Instance == null || SaveSystem.Instance == null)
            {
                Debug.LogError("Campfire Error: SpawnManager or SaveSystem is missing from the scene!");
                return;
            }

            Vector2 campfirePos = new Vector2(transform.position.x, transform.position.y);

            if (_playerInRange.currentHealth >= _playerInRange.maxHealth)
            {
                SaveSystem.Instance.SaveGame(true, campfirePos);
                PromptUI?.ShowGameSaved();
                return;
            }

            StartCoroutine(DoRest(_playerInRange, campfirePos));
        }
    }

    IEnumerator DoRest(PlayerHealth player, Vector2 campfirePos)
    {
        _isResting = true;

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

        // Destroy all existing enemies before respawning fresh ones
        foreach (EnemyMovement enemy in FindObjectsByType<EnemyMovement>(FindObjectsSortMode.None))
        {
            Destroy(enemy.gameObject);
        }

        foreach (EnemySpawner spawner in FindObjectsByType<EnemySpawner>(FindObjectsSortMode.None))
        {
            spawner.RespawnEnemies();
        }

        SaveSystem.Instance?.SaveGame(true, campfirePos);
        PromptUI?.ShowGameSaved();

        _isResting = false;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            _playerInRange = other.GetComponent<PlayerHealth>();
            if (_playerInRange != null)
            {
                PromptUI?.ShowPrompt();
                if (SkillTreePanel != null)
                    SkillTreePanel.IsNearCampfire = true;
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (SkillTreePanel != null)
            {
                SkillTreePanel.IsNearCampfire = false;
                SkillTreePanel.Hide();
            }
            PromptUI?.HidePrompt();
            _playerInRange = null;
            _isResting = false;
        }
    }
}