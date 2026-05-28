using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    public CanvasGroup canvasGroup;
    public Slider sfxSlider;
    public Toggle fullscreenToggle;
    public Button backButton;
    public PauseMenu pauseMenu;

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

    public void Open()
    {
        IsOpen = true;
        SetVisible(true);
    }

    public void Close()
    {
        IsOpen = false;
        SetVisible(false);

        var pm = pauseMenu?.GetComponent<CanvasGroup>();
        if (pm != null)
        {
            pm.alpha = 1f;
            pm.interactable = true;
            pm.blocksRaycasts = true;
        }
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
}
