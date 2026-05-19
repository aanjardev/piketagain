using UnityEngine;

/// <summary>
/// Manages the Yellow Book quest lifecycle:
/// 1. Deactivates the book at start
/// 2. Activates it when quest is accepted (Option 3)
/// 3. Cleans up when quest is completed
/// </summary>
public class YellowBookQuest : MonoBehaviour
{
    public GameObject yellowBook;

    private bool _questBookActive = false;

    void Start()
    {
        if (yellowBook != null)
        {
            yellowBook.SetActive(false);
            _questBookActive = false;
        }

        // Jika quest sudah diterima sebelumnya (misal dari load game), langsung aktifkan buku
        TryActivateYellowBook();
    }

    void Update()
    {
        // Aktifkan buku di runtime ketika quest diterima setelah Start()
        TryActivateYellowBook();

        // Jika quest sudah completed, matikan buku dan nonaktifkan script ini
        if (GameSessionManager.yellowBookQuestCompleted)
        {
            if (yellowBook != null && yellowBook.activeSelf)
            {
                yellowBook.SetActive(false);
                Debug.Log("Yellow Book Quest: Book deactivated (quest completed)");
            }

            enabled = false;
        }
    }

    private void TryActivateYellowBook()
    {
        if (yellowBook == null || _questBookActive)
            return;

        if (GameSessionManager.yellowBookQuestAccepted && !GameSessionManager.yellowBookQuestCompleted)
        {
            yellowBook.SetActive(true);
            _questBookActive = true;
            Debug.Log("Yellow Book Quest: Book activated (quest accepted)");
        }
    }
}
