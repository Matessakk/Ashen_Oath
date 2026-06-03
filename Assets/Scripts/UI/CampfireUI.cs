using System.Collections;
using UnityEngine;
using TMPro;

public class CampfireUI : MonoBehaviour
{
    [Header("UI Text Components (Assign in Inspector)")]
    public TMP_Text restPromptText;
    public TMP_Text skillPromptText;
    public TMP_Text savedNotificationText;

    [Header("References")]
    public CanvasGroup canvasGroup;

    [Header("Strings")]
    public string restString = "Press E to rest";
    public string fullHealthString = "Already rested";
    public string skillString = "Press T to open skills";
    public string savedString = "Game saved...";

    [Header("Animations")]
    public float fadeDuration = 0.2f;
    public float msgTime = 1.5f;

    Coroutine _saveMsgCoroutine;
    Coroutine _healthMsgCoroutine;

    void Awake()
    {
        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();

        if (canvasGroup != null)
            canvasGroup.alpha = 0f;

        if (restPromptText != null) restPromptText.text = restString;
        if (skillPromptText != null) skillPromptText.text = skillString;
        if (savedNotificationText != null) savedNotificationText.text = savedString;

        if (savedNotificationText != null) savedNotificationText.gameObject.SetActive(false);

        gameObject.SetActive(false);
    }

    public void ShowPrompt()
    {
        if (restPromptText != null)
        {
            restPromptText.text = restString;
            restPromptText.gameObject.SetActive(true);
        }
        if (skillPromptText != null) skillPromptText.gameObject.SetActive(true);

        gameObject.SetActive(true);
        StartCoroutine(Fade(1f));
    }

    public void HidePrompt()
    {
        if (this == null || !gameObject.activeSelf) return;
        StopAllCoroutines();
        _saveMsgCoroutine = null;
        _healthMsgCoroutine = null;
        StartCoroutine(FadeAndHide());
    }

    public void ShowGameSaved()
    {
        if (_saveMsgCoroutine != null) StopCoroutine(_saveMsgCoroutine);
        _saveMsgCoroutine = StartCoroutine(SavedNotificationSequence());
    }

    public void ShowAlreadyFull()
    {
        if (_healthMsgCoroutine != null) StopCoroutine(_healthMsgCoroutine);
        _healthMsgCoroutine = StartCoroutine(AlreadyRestedSequence());
    }

    IEnumerator SavedNotificationSequence()
    {
        if (savedNotificationText == null) yield break;

        savedNotificationText.gameObject.SetActive(true);
        yield return new WaitForSeconds(msgTime);
        savedNotificationText.gameObject.SetActive(false);
    }

    IEnumerator AlreadyRestedSequence()
    {
        if (restPromptText == null) yield break;

        restPromptText.text = fullHealthString;
        yield return new WaitForSeconds(msgTime);
        restPromptText.text = restString;
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
        if (savedNotificationText != null) savedNotificationText.gameObject.SetActive(false);
        gameObject.SetActive(false);
    }
}