using UnityEngine;
using TMPro;

public class SimpleStatsMenu : MonoBehaviour
{
    [Header("UI Panel")]
    public GameObject statsPanel;

    [Header("TMP Text Fields")]
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI xpText;
    public TextMeshProUGUI coinsText;
    public TextMeshProUGUI healthText;

    private bool isOpen = false;

    private PlayerHealth playerHealth;

    void Start()
    {
        if (statsPanel != null)
            statsPanel.SetActive(false);

        // Get reference to PlayerHealth
        playerHealth = FindObjectOfType<PlayerHealth>();
    }

    void Update()
    {
        // Toggle panel with ` key
        if (Input.GetKeyDown(KeyCode.BackQuote))
        {
            isOpen = !isOpen;

            if (statsPanel != null)
                statsPanel.SetActive(isOpen);

            Cursor.lockState = isOpen ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = isOpen;

            if (isOpen)
                UpdateUI();
        }

        // Update health text in real time
        if (playerHealth != null)
        {
            UpdateHealthUI();
        }
    }

    void UpdateUI()
    {
        if (PlayerStats.Instance != null)
        {
            if (levelText != null)
                levelText.text = $"Level: {PlayerStats.Instance.level}";

            if (xpText != null)
                xpText.text = $"XP: {PlayerStats.Instance.currentXP}/{PlayerStats.Instance.xpToNextLevel}";

            if (coinsText != null)
                coinsText.text = $"Coins: {PlayerStats.Instance.coins}";
        }

        UpdateHealthUI();
    }

    void UpdateHealthUI()
    {
        if (playerHealth == null) return;

        if (healthText != null)
        {
            healthText.text = $"{Mathf.RoundToInt(playerHealth.CurrentHealth)} / {Mathf.RoundToInt(playerHealth.MaxHealth)}";
        }
    }
}