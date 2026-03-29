using UnityEngine;
using TMPro;

public class QuestRewardDisplay : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject rewardPanel;          // Panel to show rewards
    public TextMeshProUGUI rewardText;      // TMP Text to display reward message

    [Header("Animation Settings")]
    public float displayTime = 2f;          // How long to show the reward
    private bool isShowing = false;

    void Start()
    {
        if (rewardPanel != null)
            rewardPanel.SetActive(false);
    }

    public void ShowReward(int xpAmount, int coinsAmount)
    {
        if (rewardPanel == null || rewardText == null) return;

        rewardText.text = $"+{xpAmount} XP\n+{coinsAmount} Coins";
        rewardPanel.SetActive(true);

        if (!isShowing)
            StartCoroutine(HideAfterTime());
    }

    System.Collections.IEnumerator HideAfterTime()
    {
        isShowing = true;
        yield return new WaitForSeconds(displayTime);
        if (rewardPanel != null)
            rewardPanel.SetActive(false);
        isShowing = false;
    }
}