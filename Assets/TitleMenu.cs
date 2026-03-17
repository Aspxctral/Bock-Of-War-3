using UnityEngine;

public class TitleMenu : MonoBehaviour
{
    [Header("UI References")]
    public GameObject titleUI;
    public GameObject gameUI;
    public GameObject player;

    [Header("Audio")]
    public AudioSource bgMusic; // Assign your title screen music here

    private bool hasStarted = false;

    void Start()
    {
        // Show title, hide game and player
        titleUI.SetActive(true);
        gameUI.SetActive(false);
        player.SetActive(false);

        // Play background music if assigned
        if (bgMusic != null) bgMusic.Play();

        Time.timeScale = 0f; // pause game until Play is clicked
    }

    // Call this from your Play button
    public void PlayGame()
    {
        if (hasStarted) return; // prevent double call
        hasStarted = true;

        // Disable all title UI
        titleUI.SetActive(false);
        foreach (Transform child in titleUI.transform)
            child.gameObject.SetActive(false);

        // Stop background music
        if (bgMusic != null) bgMusic.Stop();

        // Enable gameplay
        gameUI.SetActive(true);
        player.SetActive(true);
        Time.timeScale = 1f;
    }

    // Call this from your Quit button
    public void QuitGame()
    {
        Debug.Log("Quitting game...");
        UnityEditor.EditorApplication.isPlaying = false;
            Application.Quit();

    }
}