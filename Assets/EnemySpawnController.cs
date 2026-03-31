using UnityEngine;
using System.Collections.Generic;

public class QuestEnemyController : MonoBehaviour
{
    [Header("Enemy Prefab")]
    public GameObject enemyPrefab;

    [Header("Spawn Point")]
    public Transform spawnPoint;

    [Header("Patrol Points")]
    public Transform patrolPoint1;
    public Transform patrolPoint2;

    [Header("Spawn Settings")]
    public int enemiesToSpawn = 1;

    private List<GameObject> spawnedEnemies = new List<GameObject>();

    // Call this from QuestUIController to spawn enemies for the quest
    public List<GameObject> ActivateEnemies()
    {
        spawnedEnemies.Clear();

        if (enemyPrefab == null || spawnPoint == null) return spawnedEnemies;

        for (int i = 0; i < enemiesToSpawn; i++)
        {
            GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
            AssignPatrolPoints(enemy);
            spawnedEnemies.Add(enemy);
        }

        return spawnedEnemies;
    }

    private void AssignPatrolPoints(GameObject enemy)
    {
        EnemyAI ai = enemy.GetComponent<EnemyAI>();
        if (ai != null)
        {
            // Assign the two patrol points for this enemy
            ai.patrolPoints = new Transform[2];
            ai.patrolPoints[0] = patrolPoint1;
            ai.patrolPoints[1] = patrolPoint2;
        }
    }
}