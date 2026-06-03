using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class SettingsMenu : MonoBehaviour
{
    [Header("References")]
    public CanvasGroup canvasGroup;
    public Toggle fullscreenToggle;
    public Button backButton;

    [Header("Audio Controllers")]
    public AudioMixer masterMixer;
    public Slider musicSlider;
    public Slider sfxSlider;

    [Header("Volitelné — panel který se zobrazí po Back")]
    public CanvasGroup callerPanel;

    public bool IsOpen { get; private set; }

    void Awake()
    {
        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();

        SetVisible(false);

        // Safe UI Event listeners — using ?. prevents the NullReferenceException if unassigned!
        if (backButton != null)
            backButton.onClick.AddListener(Close);

        if (fullscreenToggle != null)
            fullscreenToggle.onValueChanged.AddListener(SetFullscreen);

        if (musicSlider != null)
            musicSlider.onValueChanged.AddListener(SetMusicVolume);

        if (sfxSlider != null)
            sfxSlider.onValueChanged.AddListener(SetSfxVolume);

        LoadSettings();
    }

    public void Open(CanvasGroup caller = null)
    {
        if (caller != null)
            callerPanel = caller;

        IsOpen = true;
        SetVisible(true);

        if (callerPanel != null)
            SetCanvasGroup(callerPanel, false);
    }

    public void Close()
    {
        IsOpen = false;
        SetVisible(false);

        if (callerPanel != null)
            SetCanvasGroup(callerPanel, true);
    }

    void SetMusicVolume(float value)
    {
        // Convert 0-1 linear slider values to a natural -80dB to 0dB logarithmic curve
        float dB = Mathf.Log10(Mathf.Max(value, 0.0001f)) * 20f;

        if (masterMixer != null)
            masterMixer.SetFloat("MusicVol", dB); // Must match your exposed parameter name!

        PlayerPrefs.SetFloat("MusicVolume", value);
    }

    void SetSfxVolume(float value)
    {
        float dB = Mathf.Log10(Mathf.Max(value, 0.0001f)) * 20f;

        if (masterMixer != null)
            masterMixer.SetFloat("SFXVol", dB); // Must match your exposed parameter name!

        PlayerPrefs.SetFloat("SfxVolume", value);
    }

    void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
        PlayerPrefs.SetInt("Fullscreen", isFullscreen ? 1 : 0);
    }

    void LoadSettings()
    {
        // Load & Apply Music safely
        float music = PlayerPrefs.GetFloat("MusicVolume", 0.75f);
        if (musicSlider != null) musicSlider.value = music;
        SetMusicVolume(music);

        // Load & Apply SFX safely
        float sfx = PlayerPrefs.GetFloat("SfxVolume", 0.75f);
        if (sfxSlider != null) sfxSlider.value = sfx;
        SetSfxVolume(sfx);

        // Load & Apply Screen State safely
        bool fs = PlayerPrefs.GetInt("Fullscreen", 1) == 1;
        if (fullscreenToggle != null) fullscreenToggle.isOn = fs;
        Screen.fullScreen = fs;
    }

    void SetVisible(bool visible)
    {
        if (canvasGroup == null) return;
        canvasGroup.alpha = visible ? 1f : 0f;
        canvasGroup.interactable = visible;
        canvasGroup.blocksRaycasts = visible;
    }

    void SetCanvasGroup(CanvasGroup cg, bool visible)
    {
        if (cg == null) return;
        cg.alpha = visible ? 1f : 0f;
        cg.interactable = visible;
        cg.blocksRaycasts = visible;
    }
}