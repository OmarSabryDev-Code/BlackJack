using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject rulesPanel;

    // Load the Game Scene
    public void PlayGame()
    {
        SceneManager.LoadScene("Game"); // Make sure your scene is named "Game"
    }
    public void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu"); // Make sure your scene is named "MainMenu"
    }

    // Open Rules Panel
    public void OpenRules()
    {
        if (rulesPanel != null)
            rulesPanel.SetActive(true);
    }

    // Close Rules Panel
    public void CloseRules()
    {
        if (rulesPanel != null)
            rulesPanel.SetActive(false);
    }

    // Quit the Game
    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // Stop play mode in editor
#else
            Application.Quit(); // Quit build
#endif
    }
    public void RestartGame()
    {
        SceneManager.LoadScene("Game"); // Reload the current scene
    }
}
