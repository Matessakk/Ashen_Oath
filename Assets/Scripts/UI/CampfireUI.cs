using System.Collections;
using UnityEngine;
using TMPro;


public class CampfireUI : MonoBehaviour
{
    [Header("References")]
    public TMP_Text promptText;
    public CanvasGroup canvasGroup;

    [Header("Texty")]
    public string restText = "Press E to rest";
    public string fullHealthText = "Already rested";

    [Header("Animace")]
    public float fadeDuration = 0.2f;
    public float fullMsgTime = 1.5f;

    Coroutine _fullMsgCoroutine;

    void Awake()
    {
        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();

        if (canvasGroup != null)
            canvasGroup.alpha = 0f;

        gameObject.SetActive(false);
    }

    public void ShowPrompt()
    {
        StopAllCoroutines();
        promptText.text = restText;
        gameObject.SetActive(true);
        StartCoroutine(Fade(1f));
    }

    public void HidePrompt()
    {
        if (!gameObject.activeSelf) return;
        StopAllCoroutines();
        StartCoroutine(FadeAndHide());
    }

    public void ShowAlreadyFull()
    {
        if (_fullMsgCoroutine != null)
            StopCoroutine(_fullMsgCoroutine);
        _fullMsgCoroutine = StartCoroutine(FullHealthMessage());
    }

    IEnumerator Fade(float target)
    {
        if (canvasGroup == null) yield break;

        float start = canvasGroup.alpha;
        float t = 0f;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(start, target, t / fadeDuration);
            yield return null;
        }
        canvasGroup.alpha = target;
    }

    IEnumerator FadeAndHide()
    {
        yield return StartCoroutine(Fade(0f));
        gameObject.SetActive(false);
    }

    IEnumerator FullHealthMessage()
    {
        promptText.text = fullHealthText;
        gameObject.SetActive(true);
        yield return StartCoroutine(Fade(1f));
        yield return new WaitForSeconds(fullMsgTime);
        yield return StartCoroutine(FadeAndHide());
        promptText.text = restText;
    }
}
