using UnityEngine;

public class WorldSignObject : MonoBehaviour
{
    [Header("UI Reference")]
    [Tooltip("Drag your Sign Canvas GameObject here")]
    [SerializeField] private SignCanvasUI targetUI;

    private void Start()
    {
        if (targetUI == null)
        {
            targetUI = FindFirstObjectByType<SignCanvasUI>();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && targetUI != null)
        {
            // 1. Turn the GameObject on first so Unity allows coroutines to run
            targetUI.gameObject.SetActive(true);

            // 2. Start the fade-in animation
            targetUI.OpenUI();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && targetUI != null)
        {
            // Start the fade-out animation (it will turn itself off when done)
            targetUI.CloseUI();
        }
    }
}