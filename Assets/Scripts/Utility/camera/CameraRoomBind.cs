using UnityEngine;

public class CameraRoomBind : MonoBehaviour
{
    [Header("Follow Settings")]
    public float smoothSpeed = 0.125f;
    public Vector3 offset = new Vector3(0f, 0f, -10f);

    [Header("Targeting")]
    public Transform playerTarget;

    [Header("Room Constrains")]
    [SerializeField] private CameraRoom currentRoom;

    private Camera _mainCamera;

    void Awake()
    {
        _mainCamera = GetComponent<Camera>();
        Debug.Log("[CameraRoomBind] Awake called. Setting enabled to false for loading sequence initialization.");
        enabled = false;
    }

    void OnEnable()
    {
        Debug.Log("[CameraRoomBind] Script has been ENABLED.");
    }

    private void OnDisable()
    {
        if (!gameObject.scene.isLoaded) return;

        if (playerTarget != null)
        {
            Debug.LogWarning("[CameraRoomBind] An external script tried to disable the camera track! Blocked instruction and re-enabled.");
            enabled = true;
        }
    }

    void LateUpdate()
    {
        if (playerTarget == null)
        {
            FindPlayer();
            if (playerTarget == null) return;
        }

        // Calculate basic target follow position
        Vector3 targetPosition = playerTarget.position + offset;

        // If we are currently locked inside a room, clamp the position before moving
        if (currentRoom != null)
        {
            targetPosition = ClampPosToRoom(targetPosition);
        }

        // Smooth translation
        transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed);
    }

    public void SetCurrentRoom(CameraRoom room)
    {
        currentRoom = room;
        Debug.Log($"[CameraRoomBind] Camera successfully bound to a new room layout shape.");
    }

    private Vector3 ClampPosToRoom(Vector3 targetPos)
    {
        if (_mainCamera == null) _mainCamera = GetComponent<Camera>();
        Bounds roomBounds = currentRoom.GetRoomBounds();

        // Calculate the camera's exact field-of-view dimensions in world coordinates
        float camHeight = _mainCamera.orthographicSize;
        float camWidth = camHeight * _mainCamera.aspect;

        // Clamp the camera bounds so the camera frame edges can't leak outside the room bounds box
        float minX = roomBounds.min.x + camWidth;
        float maxX = roomBounds.max.x - camWidth;
        float minY = roomBounds.min.y + camHeight;
        float maxY = roomBounds.max.y + camHeight;

        // Special handling: If the room is smaller than the camera lens window, center the camera view
        if (minX > maxX) targetPos.x = roomBounds.center.x;
        else targetPos.x = Mathf.Clamp(targetPos.x, minX, maxX);

        if (minY > maxY) targetPos.y = roomBounds.center.y;
        else targetPos.y = Mathf.Clamp(targetPos.y, minY, maxY);

        return targetPos;
    }

    public void SnapToPlayer()
    {
        Debug.Log("[CameraRoomBind] SnapToPlayer() invoked.");
        FindPlayer();

        if (playerTarget != null)
        {
            Vector3 targetPosition = playerTarget.position + offset;

            // Check if player spawns directly inside a room geometry zone
            if (currentRoom == null)
            {
                DetectRoomAtPosition(playerTarget.position);
            }

            if (currentRoom != null)
            {
                targetPosition = ClampPosToRoom(targetPosition);
            }

            transform.position = targetPosition;
            Debug.Log("[CameraRoomBind] Successfully snapped camera position to: " + transform.position);
        }
        else
        {
            Debug.LogError("[CameraRoomBind] SnapToPlayer failed because no live player target could be assigned!");
        }
    }

    private void DetectRoomAtPosition(Vector2 position)
    {
        // Fallback room scan using 2D physics overlap points on room triggers layer
        Collider2D[] colliders = Physics2D.OverlapPointAll(position);
        foreach (var col in colliders)
        {
            CameraRoom room = col.GetComponent<CameraRoom>();
            if (room != null)
            {
                SetCurrentRoom(room);
                break;
            }
        }
    }

    public void FindPlayer()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerTarget = playerObj.transform;
            Debug.Log("[CameraRoomBind] FindPlayer found active player GameObject. Target updated: " + playerObj.GetInstanceID());
        }
        else
        {
            playerTarget = null;
            Debug.LogWarning("[CameraRoomBind] FindPlayer could not find any GameObject with the 'Player' tag.");
        }
    }
}