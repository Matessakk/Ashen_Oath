using UnityEngine;
using UnityEngine.UI;


public class SettingsMenu : MonoBehaviour
{
    [Header("References")]
    public CanvasGroup canvasGroup;
    public Slider sfxSlider;
    public Toggle fullscreenToggle;
    public Button backButton;

    [Header("Volitelné — panel který se zobrazí po Back")]
    public CanvasGroup callerPanel;

    public bool IsOpen { get; private set; }

    void Awake()
    {
        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();

        SetVisible(false);

        backButton.onClick.AddListener(Close);
        sfxSlider.onValueChanged.AddListener(SetSfxVolume);
        fullscreenToggle.onValueChanged.AddListener(SetFullscreen);

        LoadSettings();
    }

    public void Open(CanvasGroup caller = null)
    {
        if (caller != null)
            callerPanel = caller;

        IsOpen = true;
        SetVisible(true);

        // skryj caller panel
        if (callerPanel != null)
            SetCanvasGroup(callerPanel, false);
    }

    public void Close()
    {
        IsOpen = false;
        SetVisible(false);

        // zobraz caller panel zpìt
        if (callerPanel != null)
            SetCanvasGroup(callerPanel, true);
    }

    void SetSfxVolume(float value)
    {
        AudioListener.volume = value;
        PlayerPrefs.SetFloat("SfxVolume", value);
    }

    void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
        PlayerPrefs.SetInt("Fullscreen", isFullscreen ? 1 : 0);
    }

    void LoadSettings()
    {
        float sfx = PlayerPrefs.GetFloat("SfxVolume", 1f);
        sfxSlider.value = sfx;
        AudioListener.volume = sfx;

        bool fs = PlayerPrefs.GetInt("Fullscreen", 1) == 1;
        fullscreenToggle.isOn = fs;
        Screen.fullScreen = fs;
    }

    void SetVisible(bool visible)
    {
        canvasGroup.alpha = visible ? 1f : 0f;
        canvasGroup.interactable = visible;
        canvasGroup.blocksRaycasts = visible;
    }

    void SetCanvasGroup(CanvasGroup cg, bool visible)
    {
        cg.alpha = visible ? 1f : 0f;
        cg.interactable = visible;
        cg.blocksRaycasts = visible;
    }
}