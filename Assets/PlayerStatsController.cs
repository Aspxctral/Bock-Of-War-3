using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats Instance;

    [Header("Player Stats")]
    public int level = 1;
    public int currentXP = 0;          // cumulative XP
    public int xpToNextLevel = 100;    // total XP required for next level
    public int coins = 0;

    public LevelUpPopup levelUpPopup;
    private PlayerHealth playerHealth;

void Start()
{
    playerHealth = FindFirstObjectByType<PlayerHealth>();

    if (playerHealth != null)
    {
        playerHealth.OnLevelUp(level);
    }
}

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Call this when player earns XP
    public void AddXP(int amount)
    {
        currentXP += amount;
        CheckLevelUp();
    }

    public void AddCoins(int amount)
    {
        coins += amount;
        CheckLevelUp();
    }

void CheckLevelUp()
{
    while (currentXP >= xpToNextLevel)
    {
        currentXP -= xpToNextLevel;
        level++;
        xpToNextLevel = Mathf.FloorToInt(xpToNextLevel * 1.2f);

        Debug.Log("Leveled up to: " + level);

        if (playerHealth != null)
        {
            playerHealth.OnLevelUp(level);
        }

           if (levelUpPopup != null)
                levelUpPopup.ShowPopup("Level Up!");
    }
}

    // NEW: returns the total XP required to reach all previous levels
    public int GetXPRequiredForPreviousLevels()
    {
        int xp = 0;
        int tempXP = 100; // starting XP requirement for level 1 → 2

        for (int i = 1; i < level; i++)
        {
            xp += tempXP;
            tempXP = Mathf.FloorToInt(tempXP * 1.2f);
        }

        return xp;
    }
}