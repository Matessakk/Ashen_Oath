using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class SignCanvasUI : MonoBehaviour
{
    [Header("Animation Settings")]
    [SerializeField] private float fadeDuration = 0.2f;

    private CanvasGroup _canvasGroup;

    void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();

        // Start completely transparent, but keep the GameObject active
        // so it can listen for triggers right away when the game boots up
        _canvasGroup.alpha = 0f;
        _canvasGroup.blocksRaycasts = false;
    }

    /// <summary>
    /// Fades in the entire UI Canvas layout.
    /// </summary>
    public void OpenUI()
    {
        StopAllCoroutines();
        _canvasGroup.blocksRaycasts = true;
        StartCoroutine(FadeUI(1f));
    }

    /// <summary>
    /// Fades out and turns off the entire UI Canvas layout.
    /// </summary>
    public void CloseUI()
    {
        StopAllCoroutines();
        _canvasGroup.blocksRaycasts = false;
        StartCoroutine(FadeAndDisable());
    }

    private IEnumerator FadeUI(float targetAlpha)
    {
        float startAlpha = _canvasGroup.alpha;
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            _canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsed / fadeDuration);
            yield return null;
        }

        _canvasGroup.alpha = targetAlpha;
    }

    private IEnumerator FadeAndDisable()
    {
        // Wait for the fade out to finish cleanly
        yield return StartCoroutine(FadeUI(0f));

        // Now it's perfectly safe to shut down the GameObject
        gameObject.SetActive(false);
    }
}