using UnityEngine;

public class CameraRoomBind : MonoBehaviour
{
    [Header("Follow Settings")]
    public float smoothSpeed = 0.125f;
    public Vector3 offset = new Vector3(0f, 0f, -10f);

    public Transform playerTarget;

    void Awake()
    {
        Debug.Log("[CameraRoomBind] Awake called. Setting enabled to false for loading sequence initialization.");
        enabled = false;
    }

    void OnEnable()
    {
        Debug.Log("[CameraRoomBind] Script has been ENABLED.");
    }

    private void OnDisable()
    {
        // If the application is quitting or changing scenes naturally, let it close safely
        if (!gameObject.scene.isLoaded) return;

        if (playerTarget != null)
        {
            Debug.LogWarning("[CameraRoomBind] An external script tried to disable the camera track! Blocked the instruction and re-enabled.");
            enabled = true;
        }
    }

    void LateUpdate()
    {
        // If our player target reference was destroyed/null, force a refresh
        if (playerTarget == null)
        {
            FindPlayer();
            if (playerTarget == null) return;
        }

        Vector3 targetPosition = playerTarget.position + offset;
        transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed);
    }

    // NEW OVERRIDE: If something else tries to disable this script while a player exists, immediately force it back on!
    

    public void SnapToPlayer()
    {
        Debug.Log("[CameraRoomBind] SnapToPlayer() invoked.");
        FindPlayer();

        if (playerTarget != null)
        {
            transform.position = playerTarget.position + offset;
            Debug.Log("[CameraRoomBind] Successfully snapped camera position to: " + transform.position);
        }
        else
        {
            Debug.LogError("[CameraRoomBind] SnapToPlayer failed because no live player target could be assigned!");
        }
    }

    public void FindPlayer()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerTarget = playerObj.transform;
            Debug.Log("[CameraRoomBind] FindPlayer found active player GameObject. Target updated to instance: " + playerObj.GetInstanceID());
        }
        else
        {
            playerTarget = null;
            Debug.LogWarning("[CameraRoomBind] FindPlayer could not find any GameObject with the 'Player' tag in the hierarchy.");
        }
    }
}