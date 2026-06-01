[System.Serializable]
public class GameData
{
    public int currentHP;
    public int maxHP;
    public float playerX;
    public float playerY;
    public int skillPoints;
    public bool fireUnlocked;
    public bool waterUnlocked;
    public bool earthUnlocked;
    public bool airUnlocked;
    public bool hpUnlocked;
    public float lastCampfireX;
    public float lastCampfireY;
    public bool hasCampfire;

    public GameData()
    {
        currentHP = 5;
        maxHP = 5;
        playerX = 0f;
        playerY = 0f;
        skillPoints = 0;
        fireUnlocked = false;
        waterUnlocked = false;
        earthUnlocked = false;
        airUnlocked = false;
        hpUnlocked = false;
    }
}