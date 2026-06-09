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

    [Header("End Screen Settings")]
    [SerializeField] private TMP_FontAsset endScreenFont;

    [Header("Sign UI Settings")]
    [SerializeField] private TMP_FontAsset signScreenFont;

    private Canvas _loadCanvas;
    private CanvasGroup _loadGroup;

    private Canvas _endCanvas;
    private CanvasGroup _endGroup;

    // --- New Sign UI Properties ---
    private Canvas _signCanvas;
    private CanvasGroup _signGroup;
    private TextMeshProUGUI _signPromptText;
    private TextMeshProUGUI _signContentText;
    private bool _isReadingSign = false;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        BuildLoadingCanvas();
        BuildEndCanvas();
        BuildSignCanvas(); // Initialize the dynamic sign canvas

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

        if (loadingScreenFont != null) label.font = loadingScreenFont;

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

    void BuildEndCanvas()
    {
        GameObject canvasGo = new GameObject("EndScreenCanvas");
        canvasGo.transform.SetParent(transform);
        _endCanvas = canvasGo.AddComponent<Canvas>();
        _endCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        _endCanvas.sortingOrder = 998;

        canvasGo.AddComponent<CanvasScaler>();
        canvasGo.AddComponent<GraphicRaycaster>();

        _endGroup = canvasGo.AddComponent<CanvasGroup>();
        _endGroup.alpha = 0f;
        _endGroup.blocksRaycasts = false;
        canvasGo.SetActive(false);

        GameObject panel = new GameObject("Background");
        panel.transform.SetParent(canvasGo.transform, false);
        Image bg = panel.AddComponent<Image>();
        bg.color = Color.black;
        RectTransform bgRect = bg.GetComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;

        GameObject textGo = new GameObject("EndText");
        textGo.transform.SetParent(canvasGo.transform, false);
        TextMeshProUGUI title = textGo.AddComponent<TextMeshProUGUI>();
        title.text = "THE END";
        title.fontSize = 60;
        title.color = Color.white;
        title.alignment = TextAlignmentOptions.Center;

        if (endScreenFont != null) title.font = endScreenFont;
        else if (loadingScreenFont != null) title.font = loadingScreenFont;

        RectTransform titleRect = title.GetComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0.5f, 0.65f);
        titleRect.anchorMax = new Vector2(0.5f, 0.65f);
        titleRect.pivot = new Vector2(0.5f, 0.5f);
        titleRect.sizeDelta = new Vector2(600f, 100f);

        GameObject btnContainer = new GameObject("ButtonContainer");
        btnContainer.transform.SetParent(canvasGo.transform, false);
        RectTransform containerRect = btnContainer.AddComponent<RectTransform>();
        containerRect.anchorMin = new Vector2(0.5f, 0.4f);
        containerRect.anchorMax = new Vector2(0.5f, 0.4f);
        containerRect.pivot = new Vector2(0.5f, 0.5f);
        containerRect.sizeDelta = new Vector2(500f, 60f);

        CreateEndButton(btnContainer.transform, "ContinueButton", "Continue Exploring", new Vector2(-130f, 0f), () => {
            StartCoroutine(FadeEndScreen(0f));
        });

        CreateEndButton(btnContainer.transform, "QuitButton", "Quit to Menu", new Vector2(130f, 0f), () => {
            StartCoroutine(FadeEndScreen(0f));
            QuitToMainMenuClean();
        });
    }

    void CreateEndButton(Transform parent, string name, string labelText, Vector2 anchoredPos, UnityEngine.Events.UnityAction onClickAction)
    {
        GameObject btnGo = new GameObject(name);
        btnGo.transform.SetParent(parent, false);

        RectTransform rect = btnGo.AddComponent<RectTransform>();
        rect.sizeDelta = new Vector2(220f, 50f);
        rect.anchoredPosition = anchoredPos;

        Image img = btnGo.AddComponent<Image>();
        img.color = new Color(0.18f, 0.18f, 0.18f, 1f);

        Button btn = btnGo.AddComponent<Button>();
        btn.onClick.AddListener(onClickAction);

        GameObject textGo = new GameObject("Text");
        textGo.transform.SetParent(btnGo.transform, false);
        TextMeshProUGUI txt = textGo.AddComponent<TextMeshProUGUI>();
        txt.text = labelText;
        txt.fontSize = 18;
        txt.color = Color.white;
        txt.alignment = TextAlignmentOptions.Center;

        if (endScreenFont != null) txt.font = endScreenFont;
        else if (loadingScreenFont != null) txt.font = loadingScreenFont;

        RectTransform txtRect = txt.GetComponent<RectTransform>();
        txtRect.anchorMin = Vector2.zero;
        txtRect.anchorMax = Vector2.one;
        txtRect.sizeDelta = Vector2.zero;
    }

    // --- Dynamic Sign Canvas Generation ---
    void BuildSignCanvas()
    {
        GameObject canvasGo = new GameObject("SignScreenCanvas");
        canvasGo.transform.SetParent(transform);
        _signCanvas = canvasGo.AddComponent<Canvas>();
        _signCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        _signCanvas.sortingOrder = 990; // Sits nicely below loading and game end screens

        canvasGo.AddComponent<CanvasScaler>();

        _signGroup = canvasGo.AddComponent<CanvasGroup>();
        _signGroup.alpha = 0f;
        _signGroup.blocksRaycasts = false;
        canvasGo.SetActive(false);

        TMP_FontAsset activeFont = signScreenFont != null ? signScreenFont : loadingScreenFont;

        // 1. Setup Interaction Indicator (e.g., "Press E to Read")
        GameObject promptGo = new GameObject("SignPromptText");
        promptGo.transform.SetParent(canvasGo.transform, false);
        _signPromptText = promptGo.AddComponent<TextMeshProUGUI>();
        _signPromptText.fontSize = 24;
        _signPromptText.color = Color.white;
        _signPromptText.alignment = TextAlignmentOptions.Center;
        if (activeFont != null) _signPromptText.font = activeFont;

        RectTransform promptRect = _signPromptText.GetComponent<RectTransform>();
        promptRect.anchorMin = new Vector2(0.5f, 0.3f);
        promptRect.anchorMax = new Vector2(0.5f, 0.3f);
        promptRect.pivot = new Vector2(0.5f, 0.5f);
        promptRect.sizeDelta = new Vector2(400f, 50f);

        // 2. Setup Main Message Box Overlay
        GameObject contentGo = new GameObject("SignContentText");
        contentGo.transform.SetParent(canvasGo.transform, false);
        _signContentText = contentGo.AddComponent<TextMeshProUGUI>();
        _signContentText.fontSize = 28;
        _signContentText.color = Color.yellow; // Distinct coloration for dialogue/lore lines
        _signContentText.alignment = TextAlignmentOptions.Center;
        if (activeFont != null) _signContentText.font = activeFont;
        _signContentText.gameObject.SetActive(false);

        RectTransform contentRect = _signContentText.GetComponent<RectTransform>();
        contentRect.anchorMin = new Vector2(0.5f, 0.5f);
        contentRect.anchorMax = new Vector2(0.5f, 0.5f);
        contentRect.pivot = new Vector2(0.5f, 0.5f);
        contentRect.sizeDelta = new Vector2(800f, 300f);
    }

    // --- Sign System API Interactions ---
    public void ShowSignPrompt(string promptText = "Press E to Read")
    {
        _isReadingSign = false;
        _signPromptText.text = promptText;
        _signPromptText.gameObject.SetActive(true);
        _signContentText.gameObject.SetActive(false);

        StopCoroutine("FadeSignScreen");
        StartCoroutine(FadeSignScreen(1f));
    }

    public void ToggleSignContent(string contentMessage)
    {
        _isReadingSign = !_isReadingSign;

        if (_isReadingSign)
        {
            _signPromptText.gameObject.SetActive(false);
            _signContentText.text = contentMessage;
            _signContentText.gameObject.SetActive(true);
        }
        else
        {
            _signContentText.gameObject.SetActive(false);
            _signPromptText.gameObject.SetActive(true);
        }
    }

    public void HideSignPrompt()
    {
        StopCoroutine("FadeSignScreen");
        StartCoroutine(FadeSignScreen(0f));
    }

    IEnumerator FadeSignScreen(float targetAlpha)
    {
        if (targetAlpha > 0f) _signCanvas.gameObject.SetActive(true);

        float startAlpha = _signGroup.alpha;
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            _signGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsed / fadeDuration);
            yield return null;
        }
        _signGroup.alpha = targetAlpha;

        if (targetAlpha <= 0f)
        {
            _signContentText.gameObject.SetActive(false);
            _signCanvas.gameObject.SetActive(false);
            _isReadingSign = false;
        }
    }

    public void ShowEndScreen()
    {
        StartCoroutine(FadeEndScreen(1f));
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

    IEnumerator FadeEndScreen(float targetAlpha)
    {
        _endCanvas.gameObject.SetActive(true);
        _endGroup.blocksRaycasts = (targetAlpha > 0f);

        float startAlpha = _endGroup.alpha;
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            _endGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsed / fadeDuration);
            yield return null;
        }
        _endGroup.alpha = targetAlpha;

        if (targetAlpha <= 0f)
            _endCanvas.gameObject.SetActive(false);
    }

    public void StartPlaythrough()
    {
        bool isNew = !SaveSystem.Instance.SaveExists();
        string targetScene = isNew ? "Game" : SaveSystem.Instance.GetSavedSceneName();
        StartCoroutine(LoadSequence(targetScene, isNew, false));
    }

    public void QuitToMainMenuClean()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null) DestroyImmediate(player);

        if (SpawnManager.Instance != null) DestroyImmediate(SpawnManager.Instance.gameObject);

        SceneManager.LoadScene("MainMenu");
    }

    public IEnumerator LocalTeleportSequence(Vector2 destination, float upwardForce = 0f)
    {
        yield return StartCoroutine(FadeLoadingScreen(1f));

        CameraRoomBind cam = FindFirstObjectByType<CameraRoomBind>();
        if (cam != null) cam.enabled = false;

        if (SpawnManager.Instance != null) SpawnManager.Instance.SpawnPlayerAtPosition(destination);

        yield return new WaitForEndOfFrame();

        cam = FindFirstObjectByType<CameraRoomBind>();
        if (cam != null)
        {
            cam.enabled = true;
            cam.SnapToPlayer();
        }

        yield return StartCoroutine(FadeLoadingScreen(0f));

        if (upwardForce > 0f)
        {
            GameObject player = SpawnManager.Instance?.ActivePlayer;
            if (player != null)
            {
                Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
                if (rb != null)
                    rb.linearVelocity = new Vector2(rb.linearVelocity.x, upwardForce);
            }
        }
    }

    public IEnumerator LoadSequence(string sceneName, bool isNewGame, bool isAreaTransition = false)
    {
        SaveSystem.Instance.CacheSkillState();
        yield return StartCoroutine(FadeLoadingScreen(1f));

        CameraRoomBind cam = FindFirstObjectByType<CameraRoomBind>();
        if (cam != null) cam.enabled = false;

        GameObject[] oldPlayers = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject oldPlayer in oldPlayers) DestroyImmediate(oldPlayer);

        AsyncOperation ao = SceneManager.LoadSceneAsync(sceneName);
        while (!ao.isDone) yield return null;
        yield return new WaitForEndOfFrame();

        foreach (EnemySpawner spawner in FindObjectsByType<EnemySpawner>(FindObjectsSortMode.None))
        {
            spawner.RespawnEnemies();
        }

        if (isNewGame)
        {
            if (SpawnManager.Instance != null) SpawnManager.Instance.SpawnFreshPlayer();
        }
        else
        {
            SaveSystem.Instance.LoadGame();
        }

        if (isAreaTransition && SpawnManager.Instance != null)
        {
            Vector2 doorTarget = SpawnManager.Instance.GetNextSpawnPoint();
            SpawnManager.Instance.SpawnPlayerAtPosition(doorTarget);
        }

        yield return new WaitForEndOfFrame();

        GameObject activePlayer = GameObject.FindGameObjectWithTag("Player");
        if (activePlayer == null && SpawnManager.Instance != null)
        {
            activePlayer = SpawnManager.Instance.ActivePlayer;
        }

        if (activePlayer != null)
        {
            SaveSystem.Instance.AssignPlayerReferences(activePlayer);

            Rigidbody2D playerRb = activePlayer.GetComponent<Rigidbody2D>();
            bool originalSimulatedState = true;
            if (playerRb != null)
            {
                originalSimulatedState = playerRb.simulated;
                playerRb.simulated = false;
            }

            cam = FindFirstObjectByType<CameraRoomBind>();
            if (cam != null)
            {
                cam.playerTarget = activePlayer.transform;
                cam.enabled = true;
                cam.SnapToPlayer();
            }

            EnemyMovement[] allEnemies = FindObjectsByType<EnemyMovement>(FindObjectsSortMode.None);
            foreach (EnemyMovement enemy in allEnemies) enemy.player = activePlayer.transform;

            BossAI boss = FindFirstObjectByType<BossAI>();
            if (boss != null) boss.player = activePlayer.transform;

            PlayerHealth hp = activePlayer.GetComponent<PlayerHealth>();
            if (hp != null) hp.onHealthChanged?.Invoke(hp.currentHealth, hp.maxHealth);

            yield return new WaitForSeconds(0.2f);

            if (playerRb != null)
            {
                playerRb.simulated = originalSimulatedState;
                playerRb.linearVelocity = Vector2.zero;
            }
        }

        yield return StartCoroutine(FadeLoadingScreen(0f));
    }
}