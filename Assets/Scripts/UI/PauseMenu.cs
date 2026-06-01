using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;



public class PauseMenu : MonoBehaviour
{
    public CanvasGroup canvasGroup;
    public Button resumeButton;
    public Button settingsButton;
    public Button quitButton;
    public SettingsMenu settingsMenu;

    bool isPaused;

    void Awake()
    {
        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();

        SetVisible(false);

        resumeButton.onClick.AddListener(Resume);
        settingsButton.onClick.AddListener(OpenSettings);
        quitButton.onClick.AddListener(Quit);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (settingsMenu.IsOpen)
            {
                settingsMenu.Close();
                return;
            }

            if (isPaused)
                Resume();
            else
                Pause();
        }
    }

    void Pause()
    {
        isPaused = true;
        Time.timeScale = 0f;
        SetVisible(true);
    }

    void Resume()
    {
        isPaused = false;
        Time.timeScale = 1f;
        SetVisible(false);
    }

    void OpenSettings()
    {
        settingsMenu.Open(canvasGroup);
    }

    void Quit()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    void SetVisible(bool visible)
    {
        canvasGroup.alpha = visible ? 1f : 0f;
        canvasGroup.interactable = visible;
        canvasGroup.blocksRaycasts = visible;
    }
}