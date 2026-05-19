using UnityEngine;
using UnityEngine.SceneManagement;
using StarterAssets;

public class NpcDialogueMenu : MonoBehaviour
{
    [Tooltip("Nama scene NPC/dialogue yang di-load additively dan akan di-unload saat pilihan dibuat")]
    public string dialogueSceneName;

    [Header("--- Door Reference ---")]
    public DoorNPCInteraction npcDoor;

    void Start()
    {
        // If this dialog scene was opened by a door in the gameplay scene,
        // the door registers itself in GameSessionManager.LastInteractedDoor.
        if (npcDoor == null && GameSessionManager.LastInteractedDoor != null)
        {
            npcDoor = GameSessionManager.LastInteractedDoor;
            Debug.Log("NpcDialogueMenu: grabbed door reference from GameSessionManager");
        }
    }

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

        // ==================== OPTION 3 = Bantu NPC ====================
        if(option == 3)
        {
            // Tampilkan quest UI
            QuestUIController.Instance.ShowQuest(
                "Quest Etika:",
                "Cari buku kuning dan kembalikan"
            );

            // Ubah pintu NPC ke return mode
            if (npcDoor != null)
            {
                npcDoor.SetReturnMode(true);
                Debug.Log("NPC door switched to return mode");
            }
        }

        CloseDialogue();
    }

    public void CloseDialogue()
    {
        FirstPersonController.SetPauseState(false);

        if (!string.IsNullOrEmpty(dialogueSceneName) && SceneManager.GetSceneByName(dialogueSceneName).isLoaded)
        {
            SceneManager.UnloadSceneAsync(dialogueSceneName);
            // Clear the door registry so it won't leak to future dialogs
            GameSessionManager.LastInteractedDoor = null;
            return;
        }

        SceneManager.UnloadSceneAsync(gameObject.scene);
        GameSessionManager.LastInteractedDoor = null;
    }
}
