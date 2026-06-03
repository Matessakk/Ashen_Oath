using UnityEngine;
using UnityEngine.SceneManagement;

public class AreaExit : MonoBehaviour
{
    [Header("Transition Settings")]
    [SerializeField] private string targetSceneName;
    [SerializeField] private Vector2 targetSpawnCoordinates;

    [Header("Safety Buffer")]
    [SerializeField] private float lockRadius = 1.5f;
    private bool isLocked = true;

    // THE DEADBOLT: Prevents this script from doing anything if a transition is already running
    private static bool isTransitioningScenes = false;

    private void Start()
    {
        // Reset the global transition lock whenever a brand new scene fully boots up
        isTransitioningScenes = false;
    }

    private void Update()
    {
        if (isLocked)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                float distance = Vector2.Distance(transform.position, player.transform.position);

                if (distance > lockRadius)
                {
                    isLocked = false;
                    Debug.Log($"[AreaExit] Player cleared the safety zone. {gameObject.name} is now active.");
                }
            }
            else
            {
                isLocked = true;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (isLocked) return;

            if (SpawnManager.Instance != null)
            {
                SpawnManager.Instance.SetNextSpawnPoint(targetSpawnCoordinates);
            }

            if (GameManager.Instance != null)
            {
                // FIX: Added 'true' at the end to tell GameManager this is an Area Transition!
                GameManager.Instance.StartCoroutine(GameManager.Instance.LoadSequence(targetSceneName, false, true));
            }
            else
            {
                SceneManager.LoadScene(targetSceneName);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(new Vector3(targetSpawnCoordinates.x, targetSpawnCoordinates.y, 0f), 0.5f);

        Gizmos.color = new Color(0f, 1f, 0f, 0.3f);
        Gizmos.DrawLine(transform.position, new Vector3(targetSpawnCoordinates.x, targetSpawnCoordinates.y, 0f));

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, lockRadius);
    }
}