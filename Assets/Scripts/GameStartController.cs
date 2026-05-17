using UnityEngine;
using UnityEngine.SceneManagement;
using StarterAssets;

public class GameStartController : MonoBehaviour
{
    [Header("--- HowToPlay Scene Settings ---")]
    [Tooltip("Nama scene HowToPlay yang akan di-load saat game dimulai")]
    public string howToPlaySceneName = "HowToPlayScreen";

    void Start()
    {
        Debug.Log("[GameStartController] Loading HowToPlayScreen as additive scene...");
        // Mirror behavior of Pause screen: set pause state so cursor unlocks and input is routed to UI
        FirstPersonController.SetPauseState(true);
        SceneManager.LoadSceneAsync(howToPlaySceneName, LoadSceneMode.Additive);
    }
}
