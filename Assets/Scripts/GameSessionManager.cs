using UnityEngine;

public static class GameSessionManager
{
    // =====================================================
    // NPC OPTION
    // =====================================================

    public static int SelectedNpcOption { get; private set; } = 0;

    // =====================================================
    // QUEST STATUS
    // =====================================================

    public static bool yellowBookQuestAccepted = false;
    public static bool yellowBookQuestCompleted = false;
    public static bool hasYellowBook = false; // Track apakah player sudah mengambil buku
    // Saat player membuka dialog via pintu, pintu itu diregister di sini
    public static DoorNPCInteraction LastInteractedDoor;

    // =====================================================
    // NPC CHOICE
    // =====================================================

    public static void SetNpcOption(int option)
    {
        SelectedNpcOption = Mathf.Clamp(option, 1, 3);

        Debug.Log($"GameSessionManager: NPC option set to {SelectedNpcOption}");

        // Kalau player pilih bantu NPC
        if (SelectedNpcOption == 3)
        {
            yellowBookQuestAccepted = true;

            Debug.Log("Yellow Book Quest Accepted");
        }
    }

    // =====================================================
    // QUEST COMPLETE
    // =====================================================

    public static void CompleteYellowBookQuest()
    {
        yellowBookQuestCompleted = true;

        Debug.Log("Yellow Book Quest Completed");
    }

    /// <summary>Dipanggil saat player mengambil yellow book</summary>
    public static void PickUpYellowBook()
    {
        hasYellowBook = true;
        Debug.Log("Yellow Book picked up!");
    }

    /// <summary>Dipanggil saat player mengembalikan yellow book</summary>
    public static void ReturnYellowBook()
    {
        hasYellowBook = false;
        Debug.Log("Yellow Book returned!");
    }

    // =====================================================
    // RESET
    // =====================================================

    public static void ResetNpcOption()
    {
        SelectedNpcOption = 0;

        yellowBookQuestAccepted = false;
        yellowBookQuestCompleted = false;
        hasYellowBook = false;

        Debug.Log("GameSessionManager: Reset");
    }

    // =====================================================
    // ENDING
    // =====================================================

    public static string GetWinSceneName()
    {
        // Ending spesial kalau bantu NPC dan quest selesai
        if (yellowBookQuestCompleted)
        {
            return "WinScreen_3";
        }

        switch (SelectedNpcOption)
        {
            case 1:
                return "WinScreen_1";

            case 2:
                return "WinScreen_2";

            case 3:
                // pilih bantu tapi quest belum selesai
                return "WinScreen_1";

            default:
                Debug.LogError("No NPC option selected before win.");
                return string.Empty;
        }
    }
}