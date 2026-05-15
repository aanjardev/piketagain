using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void StartGame()
    {
        Debug.Log("clicked start game");
        SceneManager.LoadSceneAsync("Classroom_shafwan");

    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
