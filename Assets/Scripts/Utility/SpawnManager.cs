using UnityEngine;


public class SpawnManager : MonoBehaviour
{
    public static SpawnManager Instance { get; private set; }

    [Header("References")]
    public Transform spawnPoint;
    public GameObject player;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void RespawnPlayer()
    {
        if (player == null || spawnPoint == null) return;

        player.transform.position = spawnPoint.position;

        PlayerHealth health = player.GetComponent<PlayerHealth>();
        if (health != null)
        {
            health.currentHealth = health.maxHealth;
            health.onHealthChanged?.Invoke(health.currentHealth, health.maxHealth);
        }
    }
}
