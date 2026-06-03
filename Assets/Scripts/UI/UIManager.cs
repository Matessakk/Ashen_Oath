using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("HUD Elements")]
    public UIHealth uiHealth;
    public ElementWheelUI elementWheelUI;

    [Header("UI Panels")]
    public DeathScreen deathScreen;
    public SkillTreeUI skillTreeUI;
    public CampfireUI campfireUI;
    public PauseMenu pauseMenu;
    public SettingsMenu settingsMenu;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }
}