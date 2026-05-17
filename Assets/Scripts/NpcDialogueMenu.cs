using UnityEngine;
using UnityEngine.SceneManagement;
using StarterAssets;

public class NpcDialogueMenu : MonoBehaviour
{
    [Tooltip("Nama scene NPC/dialogue yang di-load additively dan akan di-unload saat pilihan dibuat")]
    public string dialogueSceneName;

    public void ChooseOption1()
    {
        ChooseOption(1);
    }

    public void ChooseOption2()
    {
        ChooseOption(2);
    }

    public void ChooseOption3()
    {
        ChooseOption(3);
    }

    void ChooseOption(int option)
    {
        GameSessionManager.SetNpcOption(option);
        CloseDialogue();
    }

    public void CloseDialogue()
    {
        FirstPersonController.SetPauseState(false);

        if (!string.IsNullOrEmpty(dialogueSceneName) && SceneManager.GetSceneByName(dialogueSceneName).isLoaded)
        {
            SceneManager.UnloadSceneAsync(dialogueSceneName);
            return;
        }

        SceneManager.UnloadSceneAsync(gameObject.scene);
    }
}
