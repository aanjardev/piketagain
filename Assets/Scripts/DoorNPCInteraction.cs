using UnityEngine;
using UnityEngine.SceneManagement;
using StarterAssets;

public class DoorNPCInteraction : MonoBehaviour, IInteractable
{
    public string dialogueSceneName;

    public AudioSource knockSound;

    private bool canInteract = false;

    public void EnableInteraction()
    {
        canInteract = true;
    }

    public string GetPromptText()
    {
        if (!canInteract)
            return "";

        return "[E] Interaksi";
    }

    public void Interact(GameObject player)
    {
        if (!canInteract) return;

        canInteract = false;

        if (knockSound != null)
            knockSound.Stop();

        FirstPersonController.SetPauseState(true);
        SceneManager.LoadSceneAsync(dialogueSceneName, LoadSceneMode.Additive);
    }

    public void DisableInteraction()
    {
        canInteract = false;
    }

    public void OnLookAt()
    {

    }

    public void OnLookAway()
    {

    }
}