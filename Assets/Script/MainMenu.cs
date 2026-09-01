using UnityEngine;
using UnityEngine.SceneManagement; // For loading scenes

public class MainMenu : MonoBehaviour
{
    [Header("Level Settings")]
    public string firstLevelName = "TutorialLevel"; // Ensure this matches your Scene name!

    [Header("UI Panels")]
    public GameObject creditsPanel; // Drag your Credits Panel here in the Inspector

    // 1. PLAY FUNCTION
    public void PlayGame()
    {
        // Resets time scale (good habit if you have a pause menu elsewhere)
        Time.timeScale = 1f; 
        SceneManager.LoadScene(firstLevelName);
    }

    // 2. CREDITS FUNCTION
    public void ShowCredits(bool show)
    {
        if (creditsPanel != null)
        {
            creditsPanel.SetActive(show);
        }
        else
        {
            Debug.LogWarning("No Credits Panel assigned to the MenuManager!");
        }
    }

    // 3. EXIT FUNCTION
    public void ExitGame()
    {
        Debug.Log("Game is exiting...");
        Application.Quit();
        
        // Note: Application.Quit() does not close the editor, only the built game.
    }
}