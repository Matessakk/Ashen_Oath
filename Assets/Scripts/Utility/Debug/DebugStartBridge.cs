using UnityEngine;

public class DebugStartBridge : MonoBehaviour
{
    // A static variable survives scene reloads in the Unity Editor
    private static bool hasInitialSpawnTriggered = false;

    void Start()
    {
        // If we already did the initial game boot, pull the plug immediately
        if (hasInitialSpawnTriggered)
        {
            Destroy(gameObject);
            return;
        }

        if (GameManager.Instance != null)
        {
            Debug.Log("[DebugBridge] First time boot tracking initiated. Starting playthrough setup...");
            hasInitialSpawnTriggered = true;

            // Mark this object to survive long enough to start the sequence, then kill it
            DontDestroyOnLoad(gameObject);
            GameManager.Instance.StartPlaythrough();

            Destroy(gameObject); // Safely remove ourselves from the cycle
        }
        else
        {
            Debug.LogError("[DebugBridge] Could not find GameManager in scene!");
        }
    }
}