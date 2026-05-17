using UnityEngine;

public static class GameSessionManager
{
    public static int SelectedNpcOption { get; private set; } = 0;

    public static void SetNpcOption(int option)
    {
        SelectedNpcOption = Mathf.Clamp(option, 1, 3);
        Debug.Log($"GameSessionManager: NPC option set to {SelectedNpcOption}");
    }

    public static void ResetNpcOption()
    {
        SelectedNpcOption = 0;
        Debug.Log("GameSessionManager: NPC option reset");
    }

    public static string GetWinSceneName()
    {
        switch (SelectedNpcOption)
        {
            case 1: return "WinScreen_1";
            case 2: return "WinScreen_2";
            case 3: return "WinScreen_3";
            default:
                Debug.LogError("GameSessionManager: No NPC option selected before win. Please choose option 1-3.");
                return string.Empty;
        }
    }
}
