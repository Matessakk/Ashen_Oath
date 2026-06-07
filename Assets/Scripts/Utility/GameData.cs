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
    public int currentWeaponType;

    [Header("Scene & Spawn Points")]
    public string lastSavedScene; 
    public float lastCampfireX;
    public float lastCampfireY;
    public bool hasCampfire;

    
    public GameData()
    {
        skillPoints = 0;
        fireUnlocked = false;
        waterUnlocked = false;
        earthUnlocked = false;
        airUnlocked = false;
        hpUnlocked = false;

        
        maxHealth = 5;            
        
        currentWeaponType = 0;    

        lastSavedScene = "";      
        lastCampfireX = 0f;
        lastCampfireY = 0f;
        hasCampfire = false;
    }
}