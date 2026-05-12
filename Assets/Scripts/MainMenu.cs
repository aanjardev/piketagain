using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadSceneAsync("classroom");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
