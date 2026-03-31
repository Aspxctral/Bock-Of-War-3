using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats Instance;

    [Header("Player Stats")]
    public int level = 1;
    public int currentXP = 0;
    public int xpToNextLevel = 100;
    public int coins = 0;
    public int strength = 0;           // NEW: unified strength
    public int maxStrength = 1000;

    [Header("UI")]
    public Slider strengthSlider;
    public TextMeshProUGUI strengthText;

    public LevelUpPopup levelUpPopup;
    private PlayerHealth playerHealth;
    private PlayerMovement playerMovement;

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

    void Start()
    {
        playerHealth = FindFirstObjectByType<PlayerHealth>();
        playerMovement = FindFirstObjectByType<PlayerMovement>();

        if (playerHealth != null)
            playerHealth.OnLevelUp(level);

        UpdateStrengthUI();
    }

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

    public void AddStrength(int amount)
    {
        strength = Mathf.Clamp(strength + amount, 0, maxStrength);
        UpdateStrengthUI();
        Debug.Log("Strength: " + strength);
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
                playerHealth.OnLevelUp(level);

            if (levelUpPopup != null)
                levelUpPopup.ShowPopup("Level Up!");

            if (playerMovement != null)
                playerMovement.OnLevelUp(level);
        }
    }

    void UpdateStrengthUI()
    {
        if (strengthSlider != null)
            strengthSlider.value = strength;

        if (strengthText != null)
            strengthText.text = $"STR: {strength} / {maxStrength}";
    }

    public int GetXPRequiredForPreviousLevels()
    {
        int xp = 0;
        int tempXP = 100;

        for (int i = 1; i < level; i++)
        {
            xp += tempXP;
            tempXP = Mathf.FloorToInt(tempXP * 1.2f);
        }

        return xp;
    }
}