using UnityEngine;
using UnityEngine.SceneManagement;
using StarterAssets;

public class DoorNPCInteraction : MonoBehaviour, IInteractable
{
    public string dialogueSceneName;

    public AudioSource knockSound;

    private bool canInteract = false;

    // =====================================================
    // STATE: Dialog mode vs Return Book mode
    // =====================================================

    private bool isReturnMode = false;
    public bool IsReturnMode => isReturnMode;

    // =====================================================
    // INITIALIZATION
    // =====================================================

    public void EnableInteraction()
    {
        canInteract = true;
    }

    public void DisableInteraction()
    {
        canInteract = false;
    }

    // =====================================================
    // MODE SWITCHING
    // =====================================================

    /// <summary>Ketika player pilih option 3, pintu berubah ke return mode</summary>
    public void SetReturnMode(bool returnMode)
    {
        isReturnMode = returnMode;
        Debug.Log($"DoorNPCInteraction: Return mode = {isReturnMode}");
    }

    // =====================================================
    // INTERACTION
    // =====================================================

    public string GetPromptText()
    {
        if (!canInteract)
            return "";

        if (!isReturnMode)
        {
            if (GameSessionManager.hasYellowBook)
                return "";

            return "[E] Buka Pintu";
        }

        if (!GameSessionManager.yellowBookQuestAccepted || GameSessionManager.yellowBookQuestCompleted)
            return "";

        if (!GameSessionManager.hasYellowBook)
            return "";

        return "Tahan [Q] Kembalikan Buku";
    }

    public void Interact(GameObject player)
    {
        if (!canInteract) return;

        if (knockSound != null)
            knockSound.Stop();

        if (!isReturnMode)
        {
            if (GameSessionManager.hasYellowBook)
            {
                // Player is carrying the yellow book, so this door should not open.
                Debug.Log("DoorNPCInteraction: ignored interact because player is carrying yellow book.");
                return;
            }

            FirstPersonController.SetPauseState(true);
            GameSessionManager.LastInteractedDoor = this;
            SceneManager.LoadSceneAsync(dialogueSceneName, LoadSceneMode.Additive);
            return;
        }

        Debug.Log("DoorNPCInteraction: return mode active; use [Q] to return the book.");
    }

    public void OnLookAt()
    {

    }

    public void OnLookAway()
    {

    }
}
