using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public Transform player;
    public float cooldown = 0.5f;

    float nextSpawnTime;

    void Update()
    {
        if (Input.GetMouseButtonDown(1) && Time.time >= nextSpawnTime)
        {
            GameObject enemy = Instantiate(enemyPrefab, transform.position, Quaternion.identity);

            EnemyMovement em = enemy.GetComponent<EnemyMovement>();
            if (em != null)
                em.player = player;

            nextSpawnTime = Time.time + cooldown;
        }
    }
}