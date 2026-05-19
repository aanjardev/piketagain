// using UnityEngine;

// public class QuestManager : MonoBehaviour
// {
//     public static QuestManager Instance;

//     [Header("--- Quest Objects ---")]
//     public GameObject yellowBook;

//     [Header("--- Door ---")]
//     public DoorNPCInteraction npcDoor;

//     [Header("--- Checklist ---")]
//     public QuestUI questUI;

//     private bool yellowBookQuestActive = false;
//     private bool yellowBookCollected = false;

//     void Awake()
//     {
//         Instance = this;

//         // awalnya buku disembunyikan
//         if (yellowBook != null)
//             yellowBook.SetActive(false);
//     }

//     // Dipanggil saat pilih dialog bantu cari
//     public void StartYellowBookQuest()
//     {
//         yellowBookQuestActive = true;

//         // munculkan buku
//         yellowBook.SetActive(true);

//         // tambah checklist
//         questUI.ShowQuest("Cari Buku Kuning");

//         // ubah mode pintu
//         npcDoor.SetReturnMode(true);

//         Debug.Log("Quest Cari Buku Kuning Dimulai");
//     }

//     public void CollectYellowBook()
//     {
//         yellowBookCollected = true;

//         questUI.CompleteQuest();

//         Debug.Log("Buku kuning ditemukan");
//     }

//     public bool HasYellowBook()
//     {
//         return yellowBookCollected;
//     }
// }