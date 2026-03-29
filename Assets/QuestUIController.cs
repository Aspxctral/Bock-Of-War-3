using UnityEngine;
using System.Collections.Generic;

public class QuestUIController : MonoBehaviour
{
    [Header("UI")]
    public GameObject questPanel;

    [Header("Objective")]
    public GameObject worldObjective;
    public GameObject compassMarker;

    [Header("Player Movement")]
    public PlayerMovement playerMovement;

    [Header("Enemy Spawning")]
    public GameObject enemyPrefab;
    public Transform spawnPoint;
    public int enemyCount = 3;

    [Header("Quest Rewards")]
    public int questXPReward = 100;
    public int questCoinReward = 50;
    public QuestRewardDisplay rewardDisplay; // Reference to reward UI

    private bool isUIOpen = false;

    // 🔥 Track enemies
    private List<GameObject> activeEnemies = new List<GameObject>();

    void Update()
    {
        // 🔹 Toggle Quest UI
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            isUIOpen = !isUIOpen;
            questPanel.SetActive(isUIOpen);

            if (playerMovement != null)
                playerMovement.SetMovementActive(!isUIOpen);

            Cursor.lockState = isUIOpen ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = isUIOpen;
        }

        // 🔥 Check quest completion
        CheckQuestCompletion();
    }

    // 🔥 Called when player selects quest
    public void SelectQuest()
    {
        // Activate objective
        if (worldObjective != null) worldObjective.SetActive(true);
        if (compassMarker != null) compassMarker.SetActive(true);

        // Spawn enemies
        SpawnEnemies();

        // Close UI
        questPanel.SetActive(false);
        isUIOpen = false;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (playerMovement != null)
            playerMovement.SetMovementActive(true);

        Debug.Log("Quest Started! Enemies Spawned.");
    }

    void SpawnEnemies()
    {
        if (enemyPrefab == null || spawnPoint == null) return;

        activeEnemies.Clear();

        for (int i = 0; i < enemyCount; i++)
        {
            Vector3 randomOffset = new Vector3(
                Random.Range(-3f, 3f),
                0,
                Random.Range(-3f, 3f)
            );

            GameObject enemy = Instantiate(
                enemyPrefab,
                spawnPoint.position + randomOffset,
                Quaternion.identity
            );

            activeEnemies.Add(enemy);
        }
    }

    void CheckQuestCompletion()
    {
        if (activeEnemies.Count == 0) return;

        // Remove dead enemies
        activeEnemies.RemoveAll(enemy => enemy == null);

        if (activeEnemies.Count == 0)
        {
            CompleteQuest();
        }
    }

    void CompleteQuest()
    {
        Debug.Log("QUEST COMPLETE 🔥");

        // 1️⃣ Update player stats
        if (PlayerStats.Instance != null)
        {
            PlayerStats.Instance.AddXP(questXPReward);
            PlayerStats.Instance.AddCoins(questCoinReward);
        }

        // 2️⃣ Show reward UI
        if (rewardDisplay != null)
        {
            rewardDisplay.ShowReward(questXPReward, questCoinReward);
        }

        // 3️⃣ Deactivate objective
        DeactivateObjective();
    }

    public void DeactivateObjective()
    {
        if (worldObjective != null) worldObjective.SetActive(false);
        if (compassMarker != null) compassMarker.SetActive(false);
    }
}