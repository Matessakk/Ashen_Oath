using System.IO;
using UnityEngine;

/// <summary>
/// Save System — ukládá a naèítá GameData jako JSON soubor.
/// SETUP:
/// 1. Vytvoø prázdný GameObject "SaveSystem" ? pøidej tento script
/// 2. Pøiøaï PlayerHealth, playerTransform, SkillTree, SkillPointManager reference
/// 3. Volej SaveGame() u ohništì po healu
/// </summary>
public class SaveSystem : MonoBehaviour
{
    public static SaveSystem Instance { get; private set; }

    [Header("References")]
    public PlayerHealth playerHealth;
    public Transform playerTransform;
    public SkillTree skillTree;
    public SkillPointManager skillPointManager;

    string savePath => Application.persistentDataPath + "/save.json";

    void Awake()
    {
        Debug.Log("SaveSystem Awake");
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    public void SaveGame()
    {
        Debug.Log("SaveGame called");

        GameData data = new GameData();

        data.currentHP = playerHealth.currentHealth;
        data.maxHP = playerHealth.maxHealth;
        data.playerX = playerTransform.position.x;
        data.playerY = playerTransform.position.y;
        data.skillPoints = skillPointManager.skillPoints;

        data.fireUnlocked = skillTree.fireUnlocked;
        data.waterUnlocked = skillTree.waterUnlocked;
        data.earthUnlocked = skillTree.earthUnlocked;
        data.airUnlocked = skillTree.airUnlocked;
        data.hpUnlocked = skillTree.hpUnlocked;

        data.lastCampfireX = SpawnManager.Instance._lastCampfirePosition.x;
        data.lastCampfireY = SpawnManager.Instance._lastCampfirePosition.y;
        data.hasCampfire = SpawnManager.Instance._hasCampfire;

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(savePath, json);
        Debug.Log($"Game saved to {savePath}");
        Debug.Log($"Saving campfire pos: {SpawnManager.Instance._lastCampfirePosition}");
    }

    public void LoadGame()
    {
        if (!File.Exists(savePath))
        {
            
            Debug.Log("No save file found.");
            return;
        }
        
        

        string json = File.ReadAllText(savePath);
        GameData data = JsonUtility.FromJson<GameData>(json);
        
        if (data.hasCampfire)
            SpawnManager.Instance.SetCampfire(new Vector3(data.lastCampfireX, data.lastCampfireY, 0f));

        playerHealth.maxHealth = data.maxHP;
        playerHealth.currentHealth = data.currentHP;
        playerHealth.onHealthChanged?.Invoke(data.currentHP, data.maxHP);

        playerTransform.position = new Vector3(data.playerX, data.playerY, 0f);

        skillPointManager.SetPoints(data.skillPoints);

        if (data.fireUnlocked) skillTree.ForceUnlock(SkillTree.Skill.Fire);
        if (data.waterUnlocked) skillTree.ForceUnlock(SkillTree.Skill.Water);
        if (data.earthUnlocked) skillTree.ForceUnlock(SkillTree.Skill.Earth);
        if (data.airUnlocked) skillTree.ForceUnlock(SkillTree.Skill.Air);
        if (data.hpUnlocked) skillTree.ForceUnlock(SkillTree.Skill.HP);

        Debug.Log("Game loaded.");
    }

    public bool SaveExists() => File.Exists(savePath);

    public void DeleteSave()
    {
        if (File.Exists(savePath))
            File.Delete(savePath);
    }
}