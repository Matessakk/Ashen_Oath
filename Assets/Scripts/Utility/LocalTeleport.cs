using UnityEngine;

public class LocalTeleport : MonoBehaviour
{
    public enum TeleportMode { UseCoordinates, UseDestinationTransform }

    [Header("Mode Settings")]
    [SerializeField] private TeleportMode mode = TeleportMode.UseDestinationTransform;

    [Header("Targets")]
    [SerializeField] private Vector2 targetCoordinates;
    [SerializeField] private Transform targetDestinationTransform;

    [Header("Safety Cooldown")]
    [Tooltip("Time in seconds before ANY teleporter can be triggered again after a jump.")]
    [SerializeField] private float globalCooldown = 1.5f;

    // A global timestamp shared by all teleporters
    private static float nextAllowedTeleportTime = 0f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 1. Only proceed if it's the player
        if (!other.CompareTag("Player")) return;

        // 2. Check if the global cooldown has passed
        if (Time.time < nextAllowedTeleportTime)
        {
            Debug.Log($"[LocalTeleport] {gameObject.name} ignored trigger. Global cooldown active.");
            return;
        }

        Vector2 finalDestination = GetDestination();

        if (GameManager.Instance != null)
        {
            // 3. Set the future time when teleporters are allowed to work again
            // We set this BEFORE starting the sequence to immediately lock everything down
            nextAllowedTeleportTime = Time.time + globalCooldown;
            Debug.Log($"[LocalTeleport] Triggered {gameObject.name}. System locked until Time: {nextAllowedTeleportTime}");

            // 4. Fire the loading screen sequence
            GameManager.Instance.StartCoroutine(GameManager.Instance.LocalTeleportSequence(finalDestination));
        }
        else
        {
            // Fallback direct teleport if testing in an isolated scene without GameManager
            nextAllowedTeleportTime = Time.time + globalCooldown;

            if (SpawnManager.Instance != null)
                SpawnManager.Instance.SpawnPlayerAtPosition(finalDestination);
            else
                other.transform.position = new Vector3(finalDestination.x, finalDestination.y, other.transform.position.z);
        }
    }

    // REMOVED: OnTriggerExit2D completely. We no longer rely on Unity physics to unlock doors.

    public Vector2 GetDestination()
    {
        if (mode == TeleportMode.UseDestinationTransform && targetDestinationTransform != null)
        {
            return targetDestinationTransform.position;
        }
        return targetCoordinates;
    }

    private void OnDrawGizmosSelected()
    {
        Vector2 visualDest = GetDestination();
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(new Vector3(visualDest.x, visualDest.y, transform.position.z), 0.4f);
        Gizmos.DrawLine(transform.position, new Vector3(visualDest.x, visualDest.y, transform.position.z));
    }
}