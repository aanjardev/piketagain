using UnityEngine;
using UnityEngine.SceneManagement;
using StarterAssets;

public class WinMenu : MonoBehaviour
{
    [Header("Scene Names")]
    public string gameplaySceneName = "classroom";
    public string mainMenuSceneName = "HomeScreen";

    public void PlayAgain()
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