using UnityEngine;
using UnityEngine.SceneManagement;
using StarterAssets;

public class LoseMenu : MonoBehaviour
{
    [Header("Scene Names")]
    public string gameplaySceneName = "Gameplay";
    public string mainMenuSceneName = "HomeScreen";

    public void TryAgain()
    {
        FirstPersonController.SetPauseState(false);
        SceneManager.LoadScene(gameplaySceneName, LoadSceneMode.Single);
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        SceneManager.LoadScene(mainMenuSceneName, LoadSceneMode.Single);
    }
}