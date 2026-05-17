using UnityEngine;
using UnityEngine.SceneManagement;

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

        if (knockSound != null)
            knockSound.Stop();

        SceneManager.LoadScene(dialogueSceneName);
    }

    public void OnLookAt()
    {

    }

    public void OnLookAway()
    {

    }
}