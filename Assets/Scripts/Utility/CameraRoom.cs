using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class CameraRoom : MonoBehaviour
{
    private BoxCollider2D _roomCollider;

    void Awake()
    {
        _roomCollider = GetComponent<BoxCollider2D>();
        _roomCollider.isTrigger = true; // Ensure it's a trigger
    }

    // Returns the exact world boundaries of this room's collider box
    public Bounds GetRoomBounds()
    {
        if (_roomCollider == null) _roomCollider = GetComponent<BoxCollider2D>();
        return _roomCollider.bounds;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // When the player walks into this room trigger, tell the camera to bind to it
        if (other.CompareTag("Player"))
        {
            CameraRoomBind cam = Camera.main.GetComponent<CameraRoomBind>();
            if (cam != null)
            {
                cam.SetCurrentRoom(this);
            }
        }
    }
}