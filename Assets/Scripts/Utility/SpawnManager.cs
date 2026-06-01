using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager Instance { get; private set; }

    [Header("References")]
    public Transform defaultSpawnPoint;
    public GameObject player;

    public Vector3 _lastCampfirePosition;
    public bool _hasCampfire;


    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Start()
    {
        _lastCampfirePosition = defaultSpawnPoint != null
            ? defaultSpawnPoint.position
            : player.transform.position;
    }

    public void SetCampfire(Vector3 position)
    {
        _lastCampfirePosition = position;
        _hasCampfire = true;
    }

    public void RespawnPlayer()
    {
        if (player == null) return;

        player.transform.position = _hasCampfire
            ? _lastCampfirePosition
            : (defaultSpawnPoint != null ? defaultSpawnPoint.position : Vector3.zero);

        PlayerHealth health = player.GetComponent<PlayerHealth>();
        if (health != null)
        {
            health.currentHealth = health.maxHealth;
            health.onHealthChanged?.Invoke(health.currentHealth, health.maxHealth);
        }
    }
}