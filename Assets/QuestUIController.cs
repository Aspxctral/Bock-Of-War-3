using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestUIController : MonoBehaviour
{
    [Header("UI")]
    public GameObject questPanel;

    [Header("World Objectives")]
    public GameObject worldObjective1;
    public GameObject worldObjective2;
    public GameObject worldObjective3;

    [Header("Compass")]
    public CompassBar compassBar;                 // ← Drag your CompassBar here

    [Header("Player")]
    public PlayerMovement playerMovement;

    [Header("Enemy Controllers")]
    public QuestEnemyController enemyForQuest1;
    public QuestEnemyController enemyForQuest2;
    public QuestEnemyController enemyForQuest3;

    [Header("Quest Rewards")]
    public int questXPReward = 100;
    public int questCoinReward = 50;
    public QuestRewardDisplay rewardDisplay;

    [Header("Quest Buttons")]
    public Button questButton1;
    public Button questButton2;
    public Button questButton3;

    private bool isUIOpen = false;
    private List<GameObject> activeEnemies = new List<GameObject>();
    private int currentQuestIndex = 0;
    private GameObject currentWorldObjective;

    // Quest progression tracking
    private bool quest1Completed = false;
    private bool quest2Completed = false;

    void Start()
    {
        // Initially lock Quest 2 and 3
        if (questButton2 != null) questButton2.interactable = false;
        if (questButton3 != null) questButton3.interactable = false;

        // Assign button listeners
        if (questButton1 != null) questButton1.onClick.AddListener(() => StartQuest(1));
        if (questButton2 != null) questButton2.onClick.AddListener(() => StartQuest(2));
        if (questButton3 != null) questButton3.onClick.AddListener(() => StartQuest(3));

        // Hide all objectives at start
        if (worldObjective1 != null) worldObjective1.SetActive(false);
        if (worldObjective2 != null) worldObjective2.SetActive(false);
        if (worldObjective3 != null) worldObjective3.SetActive(false);
        if (compassBar != null) compassBar.ClearObjective();
    }

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

        CheckQuestCompletion();
    }

    public void StartQuest(int questNumber)
    {
        // Prevent starting locked quests
        if (questNumber == 2 && !quest1Completed) return;
        if (questNumber == 3 && !quest2Completed) return;

        currentQuestIndex = questNumber;
        activeEnemies.Clear();

        GameObject objective = null;
        QuestEnemyController enemySlot = null;

        switch (questNumber)
        {
            case 1:
                objective = worldObjective1;
                enemySlot = enemyForQuest1;
                break;
            case 2:
                objective = worldObjective2;
                enemySlot = enemyForQuest2;
                break;
            case 3:
                objective = worldObjective3;
                enemySlot = enemyForQuest3;
                break;
        }

        // Activate the correct objective
        if (objective != null)
        {
            objective.SetActive(true);
            currentWorldObjective = objective;
        }

        // Tell compass to track this objective
        if (compassBar != null)
            compassBar.SetActiveObjective(questNumber);

        // Spawn enemies for this quest
        if (enemySlot != null)
        {
            List<GameObject> spawned = enemySlot.ActivateEnemies();
            if (spawned != null && spawned.Count > 0)
                activeEnemies.AddRange(spawned);
        }

        // Close UI
        questPanel.SetActive(false);
        isUIOpen = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (playerMovement != null)
            playerMovement.SetMovementActive(true);

        Debug.Log($"Quest {questNumber} Started! Objective activated.");
    }

    void CheckQuestCompletion()
    {
        if (activeEnemies.Count == 0) return;

        activeEnemies.RemoveAll(enemy => enemy == null);

        if (activeEnemies.Count == 0)
        {
            CompleteCurrentQuest();
        }
    }

    void CompleteCurrentQuest()
    {
        Debug.Log($"Quest {currentQuestIndex} Completed!");

        // Give rewards
        if (PlayerStats.Instance != null)
        {
            PlayerStats.Instance.AddXP(questXPReward);
            PlayerStats.Instance.AddCoins(questCoinReward);
        }

        if (rewardDisplay != null)
            rewardDisplay.ShowReward(questXPReward, questCoinReward);

        // Deactivate current objective
        if (currentWorldObjective != null)
            currentWorldObjective.SetActive(false);

        if (compassBar != null)
            compassBar.ClearObjective();

        // Unlock next quest
        if (currentQuestIndex == 1)
        {
            quest1Completed = true;
            if (questButton2 != null) questButton2.interactable = true;
        }
        else if (currentQuestIndex == 2)
        {
            quest2Completed = true;
            if (questButton3 != null) questButton3.interactable = true;
        }

        activeEnemies.Clear();
        currentWorldObjective = null;
    }
}