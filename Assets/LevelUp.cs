using UnityEngine;
using TMPro;

public class LevelUpPopup : MonoBehaviour
{
    public TextMeshProUGUI popupText;
    public float moveSpeed = 30f;
    public float fadeDuration = 1f;
    public float displayTime = 0.5f;

    private CanvasGroup canvasGroup;
    private bool isActive = false;
    private float timer = 0f;
    private Vector3 startLocalPos;

    void Awake()
    {
        if (popupText != null)
        {
            canvasGroup = popupText.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = popupText.gameObject.AddComponent<CanvasGroup>();
            }

            startLocalPos = popupText.rectTransform.localPosition;
            popupText.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (!isActive) return;

        // Move up relative to start
        popupText.rectTransform.localPosition += Vector3.up * moveSpeed * Time.deltaTime;

        // Fade out
        timer += Time.deltaTime;
        if (timer > displayTime)
        {
            float fade = 1 - ((timer - displayTime) / fadeDuration);
            canvasGroup.alpha = Mathf.Clamp01(fade);
        }

        // Hide after fade completes
        if (timer > displayTime + fadeDuration)
        {
            popupText.gameObject.SetActive(false);
            popupText.rectTransform.localPosition = startLocalPos; // reset position
            isActive = false;
        }
    }

    public void ShowPopup(string text)
    {
        if (popupText == null) return;

        popupText.text = text;
        popupText.gameObject.SetActive(true);
        popupText.rectTransform.localPosition = startLocalPos; // reset to start
        canvasGroup.alpha = 1f;
        timer = 0f;
        isActive = true;
    }
}