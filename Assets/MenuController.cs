using UnityEngine;

public class MenuController : MonoBehaviour
{
    public GameObject saveMenuUI;

    private bool isOpen = false;

    void Start()
    {
        saveMenuUI.SetActive(false); // make sure it's off at start
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (isOpen)
                CloseSaveMenu();
            else
                OpenSaveMenu();
        }
    }

    public void OpenSaveMenu()
    {
        isOpen = true;

        saveMenuUI.SetActive(true);
        Time.timeScale = 0f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void CloseSaveMenu()
    {
        isOpen = false;

        saveMenuUI.SetActive(false);
        Time.timeScale = 1f;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}