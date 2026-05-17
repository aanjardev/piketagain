using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HowToPlayScreenController : MonoBehaviour
{
    [Header("--- UI References ---")]
    [Tooltip("Cancel button di Canvas > HowToPlay_panel > Cancel_btn")]
    public Button cancelBtn;

    void Awake()
    {
        if (cancelBtn != null)
        {
            cancelBtn.onClick.AddListener(OnCancelButtonPressed);
        }
        else
        {
            Debug.LogWarning("[HowToPlayScreenController] Cancel button tidak di-assign!");
        }
    }

    public void OnCancelButtonPressed()
    {
        Debug.Log("[HowToPlayScreenController] Cancel button pressed, unloading HowToPlayScreen scene...");
        // Restore player input/cursor state like Pause menu does
        StarterAssets.FirstPersonController.SetPauseState(false);
        SceneManager.UnloadSceneAsync("HowToPlayScreen");
    }

    void OnDestroy()
    {
        if (cancelBtn != null)
        {
            cancelBtn.onClick.RemoveListener(OnCancelButtonPressed);
        }
    }
}
