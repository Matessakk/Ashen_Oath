using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [Header("Buttons")]
    public Button playButton;
    public Button settingsButton;
    public Button quitButton;

    [Header("References")]
    public SettingsMenu settingsMenu;
    public CanvasGroup canvasGroup;

    [Header("Scene")]
    public string gameSceneName = "Game";

    void Awake()
    {
        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();

        playButton.onClick.AddListener(OnPlay);
        settingsButton.onClick.AddListener(OnSettings);
        quitButton.onClick.AddListener(OnQuit);
    }

    void OnPlay()
    {
        SceneManager.LoadScene(gameSceneName);
    }

    void OnSettings()
    {
        settingsMenu?.Open(canvasGroup);
    }

    void OnQuit()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}