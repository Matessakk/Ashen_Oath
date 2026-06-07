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
        // Slam the loading screen on instantly before anything else runs
        _loadGroup.alpha = 1f;
        _loadGroup.blocksRaycasts = true;
        _loadCanvas.gameObject.SetActive(true);

        bool isNew = !SaveSystem.Instance.SaveExists();
        string targetScene = isNew ? "Game" : SaveSystem.Instance.GetSavedSceneName();
        Debug.Log($"[GameManager] StartPlaythrough initialized. Target Scene: {targetScene} | IsNewGame: {isNew}");
        StartCoroutine(LoadSequence(targetScene, isNew, false));
    }

    public IEnumerator LocalTeleportSequence(Vector2 destination)
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
    }

    public IEnumerator LoadSequence(string sceneName, bool isNewGame, bool isAreaTransition = false)
    {
        Debug.Log($"[GameManager] LoadSequence started for level: {sceneName}");
        SaveSystem.Instance.CacheSkillState();

        yield return StartCoroutine(FadeLoadingScreen(1f));

        // --- STEP 1: FORCE-DESTROY OLD PLAYER CLONES IMMEDIATELY ---
        CameraRoomBind cam = FindFirstObjectByType<CameraRoomBind>();
        if (cam != null)
        {
            cam.enabled = false;
            Debug.Log("[GameManager] Disabled old camera script prior to scene dispatch.");
        }

        // Wipe out any DontDestroyOnLoad player objects instantly before transitioning
        GameObject[] oldPlayers = GameObject.FindGameObjectsWithTag("Player");
        Debug.Log($"[GameManager] Found {oldPlayers.Length} old player instances. Executing immediate cleanup...");
        foreach (GameObject oldPlayer in oldPlayers)
        {
            DestroyImmediate(oldPlayer);
        }

        // --- STEP 2: SCENE DISPATCH LOAD LOOP ---
        AsyncOperation ao = SceneManager.LoadSceneAsync(sceneName);
        while (!ao.isDone) yield return null;
        yield return new WaitForEndOfFrame();

        Debug.Log("[GameManager] Scene loaded. Spawning enemy arrays...");

        // Respawn enemies so they populate the fresh scene hierarchy layout first
        foreach (EnemySpawner spawner in FindObjectsByType<EnemySpawner>(FindObjectsSortMode.None))
        {
            spawner.RespawnEnemies();
        }

        // --- STEP 3: CREATING THE NEW PLAYER INSTANCE ---
        if (isNewGame)
        {
            Debug.Log("[GameManager] Setting up a completely fresh player instance.");
            SpawnManager.Instance.SpawnFreshPlayer();
        }
        else
        {
            Debug.Log("[GameManager] Prompting SaveSystem to build player at last saved Campfire location.");
            SaveSystem.Instance.LoadGame();
        }

        if (isAreaTransition && SpawnManager.Instance != null)
        {
            Vector2 doorTarget = SpawnManager.Instance.GetNextSpawnPoint();
            SpawnManager.Instance.SpawnPlayerAtPosition(doorTarget);
        }

        // Wait a single physical frame loop for engines to seat the instantiation parameters
        yield return new WaitForEndOfFrame();

        // --- STEP 4: RETRIEVE UNIFORM REFERENCE AND DISTRIBUTE TARGETS ---
        GameObject activePlayer = SpawnManager.Instance.ActivePlayer;
        if (activePlayer == null)
        {
            activePlayer = GameObject.FindGameObjectWithTag("Player");
        }

        if (activePlayer != null)
        {
            int freshID = activePlayer.GetInstanceID();
            Debug.Log($"[GameManager] Fresh Player verified at position: {activePlayer.transform.position}. Instance ID: {freshID}");

            SaveSystem.Instance.AssignPlayerReferences(activePlayer);

            // --- GRAVITY MAP COLLISION STABILIZATION GUARD ---
            // Briefly pause physics loop updates to prevent the player falling through asynchronous assets
            Rigidbody2D playerRb = activePlayer.GetComponent<Rigidbody2D>();
            bool originalSimulatedState = true;
            if (playerRb != null)
            {
                originalSimulatedState = playerRb.simulated;
                playerRb.simulated = false;
                Debug.Log("[GameManager] Player physics paused temporarily during environmental caching.");
            }

            // Assign camera tracking parameters
            cam = FindFirstObjectByType<CameraRoomBind>();
            if (cam != null)
            {
                cam.playerTarget = activePlayer.transform;
                cam.enabled = true;
                cam.SnapToPlayer();
            }

            // Map target values to regular enemies
            EnemyMovement[] allEnemies = FindObjectsByType<EnemyMovement>(FindObjectsSortMode.None);
            Debug.Log($"[GameManager] Re-targeting {allEnemies.Length} standard EnemyMovement instances.");
            foreach (EnemyMovement enemy in allEnemies)
            {
                enemy.player = activePlayer.transform;
            }

            // Map target values to Boss unit layouts
            BossAI boss = FindFirstObjectByType<BossAI>();
            if (boss != null)
            {
                boss.player = activePlayer.transform;
                Debug.Log("[GameManager] Core player target assigned to live BossAI element successfully.");
            }

            PlayerHealth hp = activePlayer.GetComponent<PlayerHealth>();
            if (hp != null)
                hp.onHealthChanged?.Invoke(hp.currentHealth, hp.maxHealth);

            // Give physical tile layers ample execution space to seat themselves before unfreeze
            yield return new WaitForSeconds(0.2f);

            // Restore active world gravity execution cleanly above static ground meshes
            if (playerRb != null)
            {
                playerRb.simulated = originalSimulatedState;
                playerRb.linearVelocity = Vector2.zero;
                Debug.Log("[GameManager] Physics execution restored safely.");
            }
        }
        else
        {
            Debug.LogError("[GameManager] CRITICAL CORRUPTION ERROR: activePlayer returned NULL from all query arrays!");
        }

        yield return StartCoroutine(FadeLoadingScreen(0f));
        Debug.Log("[GameManager] LoadSequence execution finished completely.");
    }
}