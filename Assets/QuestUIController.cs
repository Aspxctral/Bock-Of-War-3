using UnityEngine;

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
    public GameObject enemyPrefab;   // your enemy
    public Transform spawnPoint;     // where they spawn
    public int enemyCount = 3;

    private bool isUIOpen = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            isUIOpen = !isUIOpen;
            questPanel.SetActive(isUIOpen);

            if (playerMovement != null)
                playerMovement.SetMovementActive(!isUIOpen);

            Cursor.lockState = isUIOpen ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = isUIOpen;
        }
    }

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

        for (int i = 0; i < enemyCount; i++)
        {
            Vector3 randomOffset = new Vector3(
                Random.Range(-3f, 3f),
                0,
                Random.Range(-3f, 3f)
            );

            Instantiate(enemyPrefab, spawnPoint.position + randomOffset, Quaternion.identity);
        }
    }

    public void DeactivateObjective()
    {
        if (worldObjective != null) worldObjective.SetActive(false);
        if (compassMarker != null) compassMarker.SetActive(false);
    }
}