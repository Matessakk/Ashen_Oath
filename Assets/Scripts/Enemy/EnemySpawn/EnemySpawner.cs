using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    [Header("Enemy prefabs")]
    public List<GameObject> enemyPrefabs = new List<GameObject>();
    [HideInInspector] public Transform player;

    [Header("Spawn settings")]
    public int spawnCount = 1;
    public float spawnDelay = 1f;

    // Removed Start() auto-spawn and right-click debug spawn from Update()
    void Update() { }

    void FindPlayerInScene()
    {
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) player = p.transform;
    }

    IEnumerator SpawnEnemies()
    {
        if (player == null) FindPlayerInScene();

        for (int i = 0; i < spawnCount; i++)
        {
            if (enemyPrefabs.Count == 0) yield break;

            GameObject prefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Count)];
            GameObject enemy = Instantiate(prefab, transform.position, Quaternion.identity);

            EnemyMovement em = enemy.GetComponent<EnemyMovement>();
            if (em != null) em.player = player;

            yield return new WaitForSeconds(spawnDelay);
        }
    }

    // Called by GameManager after load sequence, and by SaveSystem on rest/save
    public void RespawnEnemies()
    {
        StartCoroutine(SpawnEnemies());
    }
}