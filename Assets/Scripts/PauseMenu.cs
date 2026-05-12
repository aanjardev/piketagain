using UnityEngine;
using UnityEngine.SceneManagement;
using StarterAssets;

public class PauseMenu : MonoBehaviour
{
    [Header("Scene Names")]
    public string pauseSceneName = "PauseScreen";
    public string gameplaySceneName = "classroom";
    public string mainMenuSceneName = "HomeScreen";

    public void Resume()
    {
        FirstPersonController.SetPauseState(false);

        if (SceneManager.GetSceneByName(pauseSceneName).isLoaded)
        {
            SceneManager.UnloadSceneAsync(pauseSceneName);
        }
    }

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

    public void QuitGame()
    {
        Time.timeScale = 1f;
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}