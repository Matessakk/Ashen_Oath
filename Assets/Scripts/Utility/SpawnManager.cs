using UnityEngine;
using UnityEngine.SceneManagement;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager Instance { get; private set; }

    [Header("Player Tracking")]
    [SerializeField] private GameObject playerPrefab;
    private GameObject activePlayer;
    public GameObject ActivePlayer => activePlayer;

    [Header("Campfire & Save Settings")]
    public Vector2 defaultSpawnPoint = new Vector2(40f, 8f);
    public Vector2 _lastCampfirePosition;
    public bool _hasCampfire;

    private Vector2 nextSpawnPoint;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void ValidateActivePlayer()
    {
        if (activePlayer == null)
        {
            activePlayer = GameObject.FindGameObjectWithTag("Player");
        }
    }

    public void SetNextSpawnPoint(Vector2 position)
    {
        nextSpawnPoint = position;
        Debug.Log("[SpawnManager] Next spawn point locked to: " + position);
    }

    public void SetCampfire(Vector2 position)
    {
        _lastCampfirePosition = position;
        _hasCampfire = true;
    }

    public void SpawnFreshPlayer()
    {
        if (activePlayer != null) Destroy(activePlayer);

        LoadPrefabFromResources();

        if (playerPrefab != null)
        {
            activePlayer = Instantiate(playerPrefab, new Vector3(defaultSpawnPoint.x, defaultSpawnPoint.y, 0f), Quaternion.identity);
        }
    }

    public GameObject SpawnPlayerAtPosition(Vector2 position)
    {
        ValidateActivePlayer();
        LoadPrefabFromResources();

        if (activePlayer == null && playerPrefab != null)
        {
            activePlayer = Instantiate(playerPrefab, new Vector3(position.x, position.y, 0f), Quaternion.identity);
            return activePlayer;
        }

        if (activePlayer != null)
        {
            activePlayer.transform.position = new Vector3(position.x, position.y, 0f);
        }

        return activePlayer;
    }

    public void RespawnPlayer()
    {
        Vector2 targetSpawn = _hasCampfire ? _lastCampfirePosition : defaultSpawnPoint;
        SpawnPlayerAtPosition(targetSpawn);

        if (activePlayer != null)
        {
            PlayerHealth health = activePlayer.GetComponent<PlayerHealth>();
            if (health != null)
            {
                health.currentHealth = health.maxHealth;
                health.onHealthChanged?.Invoke(health.currentHealth, health.maxHealth);
            }
        }
    }

    private void LoadPrefabFromResources()
    {
        if (playerPrefab == null)
        {
            playerPrefab = Resources.Load<GameObject>("Player");
        }
    }

    public Vector2 GetNextSpawnPoint()
    {
        return nextSpawnPoint;
    }
}