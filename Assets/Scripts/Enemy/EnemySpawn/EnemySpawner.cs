using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
    [Header("Enemy prefab")]
    public GameObject enemyPrefab;

    [HideInInspector] public Transform player;

    [Header("Spawn settings")]
    public int spawnCount = 1;
    public float spawnDelay = 1f;

    void Update() 
    {
        if (player == null)
        {
            GameObject freshPlayer = GameObject.FindGameObjectWithTag("Player");
            if (freshPlayer != null) player = freshPlayer.transform;
            else return;
        }
    }

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
            if (enemyPrefab == null) yield break;
            GameObject enemy = Instantiate(enemyPrefab, transform.position, Quaternion.identity);
            EnemyMovement em = enemy.GetComponent<EnemyMovement>();
            if (em != null) em.player = player;
            yield return new WaitForSeconds(spawnDelay);
        }
    }

    public void RespawnEnemies()
    {
        StartCoroutine(SpawnEnemies());
    }
}