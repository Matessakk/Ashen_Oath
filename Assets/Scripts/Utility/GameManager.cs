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
        Debug.Log($"[GameManager] StartPlaythrough initialized. Target Scene: {targetScene} | IsNewGame: {isNew}");
        StartCoroutine(LoadSequence(targetScene, isNew, false));
    }

    public void QuitToMainMenuClean()
    {
        Debug.Log("[GameManager] Initiating hard cleanup sequence for Main Menu exit...");

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            DestroyImmediate(player);
            Debug.Log("[GameManager] Persistent player reference permanently purged from hierarchy.");
        }

        if (SpawnManager.Instance != null)
        {
            DestroyImmediate(SpawnManager.Instance.gameObject);
            Debug.Log("[GameManager] SpawnManager instance destroyed to clear internal reference caching.");
        }

        SceneManager.LoadScene("MainMenu");
    }

    public IEnumerator LocalTeleportSequence(Vector2 destination, float upwardForce = 0f)
    {
        yield return StartCoroutine(FadeLoadingScreen(1f));

        CameraRoomBind cam = FindFirstObjectByType<CameraRoomBind>();
        if (cam != null) cam.enabled = false;

        if (SpawnManager.Instance != null)
        {
            SpawnManager.Instance.SpawnPlayerAtPosition(destination);
        }

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
        Debug.Log($"[GameManager] LoadSequence started for level: {sceneName}");
        SaveSystem.Instance.CacheSkillState();

        yield return StartCoroutine(FadeLoadingScreen(1f));

        // --- STEP 1: PRE-DISPATCH ABSOLUTE PURGE ---
        CameraRoomBind cam = FindFirstObjectByType<CameraRoomBind>();
        if (cam != null) cam.enabled = false;

        GameObject[] oldPlayers = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject oldPlayer in oldPlayers)
        {
            DestroyImmediate(oldPlayer);
        }

        // --- STEP 2: LOAD NEW WORLD SCENE ---
        AsyncOperation ao = SceneManager.LoadSceneAsync(sceneName);
        while (!ao.isDone) yield return null;
        yield return new WaitForEndOfFrame();

        foreach (EnemySpawner spawner in FindObjectsByType<EnemySpawner>(FindObjectsSortMode.None))
        {
            spawner.RespawnEnemies();
        }

        // --- STEP 3: CONSTRUCT LIVE PLAYER THROUGH CLEAN PIPELINE ---
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

        // --- STEP 4: MANDATORY HIERARCHY EVALUATION ---
        GameObject activePlayer = GameObject.FindGameObjectWithTag("Player");
        if (activePlayer == null && SpawnManager.Instance != null)
        {
            activePlayer = SpawnManager.Instance.ActivePlayer;
        }

        if (activePlayer != null)
        {
            Debug.Log($"[GameManager] Active Player tracked successfully at: {activePlayer.transform.position}");
            SaveSystem.Instance.AssignPlayerReferences(activePlayer);

            Rigidbody2D playerRb = activePlayer.GetComponent<Rigidbody2D>();
            bool originalSimulatedState = true;
            if (playerRb != null)
            {
                originalSimulatedState = playerRb.simulated;
                playerRb.simulated = false;
            }

            // Bind Camera Room System
            cam = FindFirstObjectByType<CameraRoomBind>();
            if (cam != null)
            {
                cam.playerTarget = activePlayer.transform;
                cam.enabled = true;
                cam.SnapToPlayer();
            }

            // Bind Normal Enemies
            EnemyMovement[] allEnemies = FindObjectsByType<EnemyMovement>(FindObjectsSortMode.None);
            foreach (EnemyMovement enemy in allEnemies)
            {
                enemy.player = activePlayer.transform;
            }

            // Bind Boss AI
            BossAI boss = FindFirstObjectByType<BossAI>();
            if (boss != null)
            {
                boss.player = activePlayer.transform;
                Debug.Log("[GameManager] BossAI bound to verified player object.");
            }

            PlayerHealth hp = activePlayer.GetComponent<PlayerHealth>();
            if (hp != null)
                hp.onHealthChanged?.Invoke(hp.currentHealth, hp.maxHealth);

            yield return new WaitForSeconds(0.2f);

            if (playerRb != null)
            {
                playerRb.simulated = originalSimulatedState;
                playerRb.linearVelocity = Vector2.zero;
            }
        }
        else
        {
            Debug.LogError("[GameManager] CRITICAL CORRUPTION: No player object found via FindGameObjectWithTag or SpawnManager!");
        }

        yield return StartCoroutine(FadeLoadingScreen(0f));
    }
}