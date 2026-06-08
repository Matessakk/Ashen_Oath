using System.IO;
using UnityEngine;

public class SaveSystem : MonoBehaviour
{
    public static SaveSystem Instance { get; private set; }

    [Header("Runtime Injected References")]
    public PlayerHealth playerHealth;
    public Transform playerTransform;

    [Header("Global Scene References")]
    public SkillTree skillTree;
    public SkillPointManager skillPointManager;

    // Cached skill state — survives scene transitions even if the
    // SkillTree/SkillPointManager objects get destroyed
    private int _cachedSkillPoints;
    private bool _cachedFire, _cachedWater, _cachedEarth, _cachedAir, _cachedHp;

    string savePath => Application.persistentDataPath + "/save.json";

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // Call this whenever skill state changes (on unlock, on point spend, etc.)
    // GameManager should also call this right before any scene load
    public void CacheSkillState()
    {
        if (skillPointManager != null)
            _cachedSkillPoints = skillPointManager.skillPoints;

        if (skillTree != null)
        {
            _cachedFire = skillTree.fireUnlocked;
            _cachedWater = skillTree.waterUnlocked;
            _cachedEarth = skillTree.earthUnlocked;
            _cachedAir = skillTree.airUnlocked;
            _cachedHp = skillTree.hpUnlocked;
        }

        Debug.Log("[SaveSystem] Skill state cached.");
    }

    public void AssignPlayerReferences(GameObject playerGo)
    {
        playerTransform = playerGo.transform;
        playerHealth = playerGo.GetComponent<PlayerHealth>();
    }

    public bool SaveExists() => File.Exists(savePath);

    public string GetSavedSceneName()
    {
        if (!SaveExists()) return "Game";
        string json = File.ReadAllText(savePath);
        GameData data = JsonUtility.FromJson<GameData>(json);
        return string.IsNullOrEmpty(data.lastSavedScene) ? "Game" : data.lastSavedScene;
    }

    public void SaveGame(bool updatingCampfire = false, Vector2 freshCampfirePos = default)
    {
        // Always snapshot live references first if they're still alive,
        // then fall back to the cache if they've already been destroyed
        CacheSkillState();

        GameData data = new GameData();

        // Write skill points from cache
        data.skillPoints = _cachedSkillPoints;

        // Write skill unlocks from cache
        data.fireUnlocked = _cachedFire;
        data.waterUnlocked = _cachedWater;
        data.earthUnlocked = _cachedEarth;
        data.airUnlocked = _cachedAir;
        data.hpUnlocked = _cachedHp;

        // Player health still comes from the live reference (it's runtime-injected)
        if (playerHealth != null)
            data.maxHealth = playerHealth.maxHealth;

        if (updatingCampfire)
        {
            data.lastCampfireX = freshCampfirePos.x;
            data.lastCampfireY = freshCampfirePos.y;
            data.hasCampfire = true;
            if (SpawnManager.Instance != null)
            {
                SpawnManager.Instance.SetCampfire(freshCampfirePos);
            }
        }
        else
        {
            if (SpawnManager.Instance != null)
            {
                data.lastCampfireX = SpawnManager.Instance._lastCampfirePosition.x;
                data.lastCampfireY = SpawnManager.Instance._lastCampfirePosition.y;
                data.hasCampfire = SpawnManager.Instance._hasCampfire;
            }
        }

        data.lastSavedScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(savePath, json);
        Debug.Log($"[SaveSystem] Saved game data successfully to: {savePath}");
    }

    public void LoadGame()
    {
        // FIX: Instead of returning out completely and causing a black screen crash, 
        // fallback to spawning a standard player instance if the file is missing!
        if (!File.Exists(savePath))
        {
            Debug.LogWarning("[SaveSystem] LoadGame invoked but no JSON save profile exists. Spawning fallback default player.");
            if (SpawnManager.Instance != null)
            {
                SpawnManager.Instance.SpawnFreshPlayer();
            }
            return;
        }

        string json = File.ReadAllText(savePath);
        GameData data = JsonUtility.FromJson<GameData>(json);

        if (SpawnManager.Instance != null)
        {
            if (data.hasCampfire)
            {
                SpawnManager.Instance._hasCampfire = true;
                SpawnManager.Instance._lastCampfirePosition = new Vector2(data.lastCampfireX, data.lastCampfireY);
            }

            Vector3 targetSpawnPosition = data.hasCampfire
                ? new Vector3(data.lastCampfireX, data.lastCampfireY, 0f)
                : (Vector3)SpawnManager.Instance.defaultSpawnPoint;

            GameObject spawnedPlayer = SpawnManager.Instance.SpawnPlayerAtPosition(targetSpawnPosition);
            if (spawnedPlayer != null)
            {
                AssignPlayerReferences(spawnedPlayer);
            }
        }

        if (playerHealth != null)
        {
            if (data.maxHealth > 0) playerHealth.maxHealth = data.maxHealth;
            playerHealth.currentHealth = playerHealth.maxHealth;
            playerHealth.onHealthChanged?.Invoke(playerHealth.currentHealth, playerHealth.maxHealth);
        }

        if (skillPointManager != null) skillPointManager.SetPoints(data.skillPoints);

        if (skillTree != null)
        {
            if (data.fireUnlocked) skillTree.ForceUnlock(SkillTree.Skill.Fire);
            if (data.waterUnlocked) skillTree.ForceUnlock(SkillTree.Skill.Water);
            if (data.earthUnlocked) skillTree.ForceUnlock(SkillTree.Skill.Earth);
            if (data.airUnlocked) skillTree.ForceUnlock(SkillTree.Skill.Air);
            if (data.hpUnlocked) skillTree.ForceUnlock(SkillTree.Skill.HP);
        }

        // Update cache to match what was just loaded
        _cachedSkillPoints = data.skillPoints;
        _cachedFire = data.fireUnlocked;
        _cachedWater = data.waterUnlocked;
        _cachedEarth = data.earthUnlocked;
        _cachedAir = data.airUnlocked;
        _cachedHp = data.hpUnlocked;
    }
}