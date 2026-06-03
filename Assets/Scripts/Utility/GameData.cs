using UnityEngine;

[System.Serializable]
public class GameData
{
    [Header("Skill Tree & Upgrades")]
    public int skillPoints;
    public bool fireUnlocked;
    public bool waterUnlocked;
    public bool earthUnlocked;
    public bool airUnlocked;
    public bool hpUnlocked;

    [Header("Player Vital Stats")]
    public int maxHealth;         

    [Header("Weapon States")]
    public int currentWeaponType; // Stores which weapon was equipped (Sword/Bow)

    [Header("Scene & Spawn Points")]
    public string lastSavedScene; // Saves which room/level you were last standing in
    public float lastCampfireX;
    public float lastCampfireY;
    public bool hasCampfire;

    /// <summary>
    /// Default values used when starting a completely brand new game slot.
    /// </summary>
    public GameData()
    {
        skillPoints = 0;
        fireUnlocked = false;
        waterUnlocked = false;
        earthUnlocked = false;
        airUnlocked = false;
        hpUnlocked = false;

        // Default base starting stats before purchasing upgrades
        maxHealth = 5;            // Matches your PlayerHealth base maxHealth
        
        currentWeaponType = 0;    // Default weapon index (e.g., Sword)

        lastSavedScene = "";      // Will be filled automatically by your SaveSystem
        lastCampfireX = 0f;
        lastCampfireY = 0f;
        hasCampfire = false;
    }
}