using System.Collections;
using UnityEngine;

public class SceneTransitionUI : MonoBehaviour
{
    public static SceneTransitionUI Instance { get; private set; }

    private CanvasGroup fadeCanvasGroup;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Auto-detect the component so you don't need an inspector slot
        fadeCanvasGroup = GetComponentInChildren<CanvasGroup>();

        if (fadeCanvasGroup == null)
        {
            Debug.LogError("[SceneTransitionUI] Missing CanvasGroup component on this object or its children!");
        }
    }

    public IEnumerator Fade(float targetAlpha, float duration)
    {
        if (fadeCanvasGroup == null) yield break;

        float startAlpha = fadeCanvasGroup.alpha;
        float time = 0;

        while (time < duration)
        {
            time += Time.deltaTime;
            fadeCanvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, time / duration);
            yield return null;
        }

        fadeCanvasGroup.alpha = targetAlpha;
    }
}