using UnityEngine;

public class CameraRoomBind : MonoBehaviour
{
    [Header("Follow Settings")]
    public float smoothSpeed = 0.125f;
    public Vector3 offset = new Vector3(0f, 0f, -10f);

    private Transform playerTarget;

    void Start()
    {
        // Camera starts disabled — GameManager re-enables it after the player spawns
        enabled = false;
    }

    void LateUpdate()
    {
        if (playerTarget == null)
        {
            FindPlayer();
            if (playerTarget == null) return;
        }

        Vector3 targetPosition = playerTarget.position + offset;
        transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed);
    }

    // Called by GameManager right after player spawns, while screen is still black
    public void SnapToPlayer()
    {
        FindPlayer();
        if (playerTarget != null)
            transform.position = playerTarget.position + offset;
    }

    void FindPlayer()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            playerTarget = playerObj.transform;
    }
}