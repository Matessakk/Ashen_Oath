using System.Collections;
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

    [Header("Upward Exit Settings")]
    [Tooltip("Enable this for floor-level exits that launch the player upward into the next area.")]
    [SerializeField] private bool isUpwardExit = false;
    [Tooltip("How much upward velocity to give the player on the other side.")]
    [SerializeField] private float exitUpwardForce = 12f;

    private static float nextAllowedTeleportTime = 0f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        if (Time.time < nextAllowedTeleportTime)
        {
            Debug.Log($"[LocalTeleport] {gameObject.name} ignored trigger. Global cooldown active.");
            return;
        }

        Vector2 finalDestination = GetDestination();

        if (GameManager.Instance != null)
        {
            nextAllowedTeleportTime = Time.time + globalCooldown;
            Debug.Log($"[LocalTeleport] Triggered {gameObject.name}. System locked until Time: {nextAllowedTeleportTime}");

            if (isUpwardExit)
                GameManager.Instance.StartCoroutine(GameManager.Instance.LocalTeleportSequence(finalDestination, exitUpwardForce));
            else
                GameManager.Instance.StartCoroutine(GameManager.Instance.LocalTeleportSequence(finalDestination));
        }
        else
        {
            nextAllowedTeleportTime = Time.time + globalCooldown;
            if (SpawnManager.Instance != null)
                SpawnManager.Instance.SpawnPlayerAtPosition(finalDestination);
            else
                other.transform.position = new Vector3(finalDestination.x, finalDestination.y, other.transform.position.z);
        }
    }

    public Vector2 GetDestination()
    {
        if (mode == TeleportMode.UseDestinationTransform && targetDestinationTransform != null)
            return targetDestinationTransform.position;

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