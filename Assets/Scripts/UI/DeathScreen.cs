using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DeathScreen : MonoBehaviour
{
    [Header("References")]
    public CanvasGroup canvasGroup;
    public TMP_Text youDiedText;
    public Button respawnButton;
    public Button quitButton;

    [Header("Animace")]
    public float fadeDuration = 1.2f;
    public float textDelay = 0.5f;

    void Awake()
    {
        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();

        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        if (youDiedText != null)
            youDiedText.alpha = 0f;

        gameObject.SetActive(false);

        respawnButton.onClick.AddListener(OnRespawn);
        quitButton.onClick.AddListener(OnQuit);
    }

    public void Show()
    {
        gameObject.SetActive(true);
        StartCoroutine(FadeIn());
    }

    public void Hide()
    {
        StartCoroutine(FadeOutAndDisable());
    }

    IEnumerator FadeIn()
    {
        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;
            canvasGroup.alpha = Mathf.Clamp01(t / fadeDuration);
            yield return null;
        }
        canvasGroup.alpha = 1f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;

        yield return new WaitForSecondsRealtime(textDelay);

        t = 0f;
        float textFade = 0.4f;
        while (t < textFade)
        {
            t += Time.unscaledDeltaTime;
            youDiedText.alpha = Mathf.Clamp01(t / textFade);
            yield return null;
        }
        youDiedText.alpha = 1f;
    }

    IEnumerator FadeOutAndDisable()
    {
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        float t = fadeDuration;
        while (t > 0f)
        {
            t -= Time.unscaledDeltaTime;
            canvasGroup.alpha = Mathf.Clamp01(t / fadeDuration);
            yield return null;
        }
        canvasGroup.alpha = 0f;
        if (youDiedText != null) youDiedText.alpha = 0f;
        gameObject.SetActive(false);
    }

    void OnRespawn()
    {
        Hide();
        Time.timeScale = 1f;
        SpawnManager.Instance?.RespawnPlayer();

        GameObject player = SpawnManager.Instance?.ActivePlayer;
        if (player != null)
        {
            if (player.TryGetComponent<PlayerMovement>(out PlayerMovement pm)) pm.enabled = true;
            if (player.TryGetComponent<PlayerAttack>(out PlayerAttack pa)) pa.enabled = true;
        }
    }

    void OnQuit()
    {
        Time.timeScale = 1f;
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}