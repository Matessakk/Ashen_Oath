using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    [Header("Enemy prefabs")]
    public List<GameObject> enemyPrefabs = new List<GameObject>();

    public Transform player;

    [Header("Spawn settings")]
    public int spawnCount = 1;
    public float spawnDelay = 1f;

    void Start()
    {
        StartCoroutine(SpawnEnemies());   
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            StartCoroutine(SpawnEnemies());
        }
    }

    IEnumerator SpawnEnemies()
    {
        for (int i = 0; i < spawnCount; i++)
        {
            if (enemyPrefabs.Count == 0) yield break;

            GameObject prefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Count)];
            GameObject enemy = Instantiate(prefab, transform.position, Quaternion.identity);

            EnemyMovement em = enemy.GetComponent<EnemyMovement>();
            if (em != null)
                em.player = player;

            yield return new WaitForSeconds(spawnDelay);
        }
    }

    public void RespawnEnemies()
    {
        StartCoroutine(SpawnEnemies());
    }
}
