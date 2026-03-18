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
        if (hasStarted) return;
        hasStarted = true;

        // Hide Title UI
        titleUI.SetActive(false);

        // Show gameplay
        gameUI.SetActive(true);
        player.SetActive(true);

        // Lock the cursor here (this will always run)
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Stop background music if needed
        if (bgMusic != null) bgMusic.Stop();

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