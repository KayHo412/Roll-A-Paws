using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.InputSystem;

public class SceneManagement : MonoBehaviour
{
    [SerializeField] private GameObject startMenuPanel;
    [SerializeField] private GameObject nextLevelButton;

    private static bool isRestarting = false;

    private void Start()
    {
        // Hide the next level button by default when any level boots up
        if (nextLevelButton != null)
        {
            nextLevelButton.SetActive(false);
        }

        // ONLY skip the start menu if the player specifically hit the restart key 'R'
        if (isRestarting)
        {
            Time.timeScale = 1f;
            if (startMenuPanel != null) startMenuPanel.SetActive(false);
            isRestarting = false; // Reset the flag
        }
        else
        {
            // Fresh launch OR advancing to Level 2: Freeze time and show the start panel!
            Time.timeScale = 0f;
            if (startMenuPanel != null) startMenuPanel.SetActive(true);
        }
    }

    private void Update()
    {
        if (Keyboard.current != null)
        {
            if (Keyboard.current.rKey.wasPressedThisFrame) RestartGame();
            if (Keyboard.current.mKey.wasPressedThisFrame) OpenMainMenu();
        }
    }

    public void StartGame()
    {
        Time.timeScale = 1f;
        if (startMenuPanel != null) startMenuPanel.SetActive(false);
    }

    public void RestartGame()
    {
        isRestarting = true;
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void LoadNextLevel()
    {
        Time.timeScale = 1f;
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;

        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            // Simply load the scene. Awake() will automatically handle showing the panel
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            Debug.LogWarning("No more levels found! Returning to Main Menu.");
            OpenMainMenu();
        }
    }

    public void ShowNextLevelButton()
    {
        if (nextLevelButton != null)
        {
            nextLevelButton.SetActive(true);
        }
    }

    public void OpenMainMenu()
{
    // 1. Force the restart flag to false so the scene knows to show the start menu on reload
    isRestarting = false;

    // 2. Unfreeze the clock engine so the reload process goes through smoothly
    Time.timeScale = 1f;

    // 3. Fully reload the active scene layout from scratch!
    // This wipes out all collected coin counts, respawns items, and clears the win/lose panel.
    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
}
}