using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Loading Screen")]
    [SerializeField] private float fadeDuration = 0.5f;
    [SerializeField] private TMP_FontAsset loadingScreenFont;

    private Canvas _loadCanvas;
    private CanvasGroup _loadGroup;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        BuildLoadingCanvas();

        _loadGroup.alpha = 1f;
        _loadGroup.blocksRaycasts = true;
        _loadCanvas.gameObject.SetActive(true);
    }

    void BuildLoadingCanvas()
    {
        GameObject canvasGo = new GameObject("LoadingCanvas");
        canvasGo.transform.SetParent(transform);
        _loadCanvas = canvasGo.AddComponent<Canvas>();
        _loadCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        _loadCanvas.sortingOrder = 999;

        canvasGo.AddComponent<CanvasScaler>();
        canvasGo.AddComponent<GraphicRaycaster>();

        _loadGroup = canvasGo.AddComponent<CanvasGroup>();
        _loadGroup.alpha = 1f;
        _loadGroup.blocksRaycasts = true;

        GameObject panel = new GameObject("Panel");
        panel.transform.SetParent(canvasGo.transform, false);
        Image bg = panel.AddComponent<Image>();
        bg.color = Color.black;
        RectTransform bgRect = bg.GetComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.offsetMin = Vector2.zero;
        bgRect.offsetMax = Vector2.zero;

        GameObject textGo = new GameObject("LoadingText");
        textGo.transform.SetParent(canvasGo.transform, false);

        TextMeshProUGUI label = textGo.AddComponent<TextMeshProUGUI>();
        label.text = "Loading...";

        if (loadingScreenFont != null)
        {
            label.font = loadingScreenFont;
        }
        else
        {
            Debug.LogWarning("[GameManager] No TMP Loading Screen Font assigned in Inspector! Using TMP default.");
        }

        label.fontSize = 40;
        label.color = Color.white;
        label.alignment = TextAlignmentOptions.BottomLeft;

        RectTransform labelRect = label.GetComponent<RectTransform>();
        labelRect.anchorMin = new Vector2(0f, 0f);
        labelRect.anchorMax = new Vector2(0f, 0f);
        labelRect.pivot = new Vector2(0f, 0f);
        labelRect.sizeDelta = new Vector2(300f, 60f);
        labelRect.anchoredPosition = new Vector2(40f, 40f);
    }

    IEnumerator FadeLoadingScreen(float targetAlpha)
    {
        _loadCanvas.gameObject.SetActive(true);
        _loadGroup.blocksRaycasts = (targetAlpha > 0f);

        float startAlpha = _loadGroup.alpha;
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            _loadGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsed / fadeDuration);
            yield return null;
        }
        _loadGroup.alpha = targetAlpha;

        if (targetAlpha <= 0f)
            _loadCanvas.gameObject.SetActive(false);
    }

    public void StartPlaythrough()
    {
        bool isNew = !SaveSystem.Instance.SaveExists();
        string targetScene = isNew ? "Game" : SaveSystem.Instance.GetSavedSceneName();
        StartCoroutine(LoadSequence(targetScene, isNew, false));
    }

    /// <summary>
    /// Handles shifting the player across areas in the EXACT SAME scene smoothly.
    /// </summary>
    public IEnumerator LocalTeleportSequence(Vector2 destination)
    {
        yield return StartCoroutine(FadeLoadingScreen(1f));

        CameraRoomBind cam = FindFirstObjectByType<CameraRoomBind>();
        if (cam != null) cam.enabled = false;

        if (SpawnManager.Instance != null)
        {
            SpawnManager.Instance.SpawnPlayerAtPosition(destination);
            Debug.Log("[GameManager] Local teleported player to " + destination);
        }

        yield return new WaitForEndOfFrame();

        cam = FindFirstObjectByType<CameraRoomBind>();
        if (cam != null)
        {
            cam.enabled = true;
            cam.SnapToPlayer();
        }

        yield return StartCoroutine(FadeLoadingScreen(0f));
    }

    /// <summary>
    /// Handles full asynchronous loads between completely separate Unity scenes.
    /// </summary>
    public IEnumerator LoadSequence(string sceneName, bool isNewGame, bool isAreaTransition = false)
    {
        SaveSystem.Instance.CacheSkillState();

        yield return StartCoroutine(FadeLoadingScreen(1f));

        CameraRoomBind cam = FindFirstObjectByType<CameraRoomBind>();
        if (cam != null) cam.enabled = false;

        AsyncOperation ao = SceneManager.LoadSceneAsync(sceneName);
        while (!ao.isDone) yield return null;
        yield return new WaitForEndOfFrame();

        if (isNewGame)
            SpawnManager.Instance.SpawnFreshPlayer();
        else
            SaveSystem.Instance.LoadGame();

        if (isAreaTransition && SpawnManager.Instance != null)
        {
            Vector2 doorTarget = SpawnManager.Instance.GetNextSpawnPoint();
            SpawnManager.Instance.SpawnPlayerAtPosition(doorTarget);
            Debug.Log("[GameManager] Area transition: moved player to " + doorTarget);
        }

        cam = FindFirstObjectByType<CameraRoomBind>();
        if (cam != null)
        {
            cam.enabled = true;
            cam.SnapToPlayer();
        }

        if (SpawnManager.Instance.ActivePlayer != null)
        {
            SaveSystem.Instance.AssignPlayerReferences(SpawnManager.Instance.ActivePlayer);
            PlayerHealth hp = SpawnManager.Instance.ActivePlayer.GetComponent<PlayerHealth>();
            if (hp != null)
                hp.onHealthChanged?.Invoke(hp.currentHealth, hp.maxHealth);
        }

        foreach (EnemySpawner spawner in FindObjectsByType<EnemySpawner>(FindObjectsSortMode.None))
            spawner.RespawnEnemies();

        yield return StartCoroutine(FadeLoadingScreen(0f));
    }
}