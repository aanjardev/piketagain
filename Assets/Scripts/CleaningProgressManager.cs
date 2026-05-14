using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Tracks global cleaning progress (books shelved, trash binned, dust wiped).
/// Attach to any persistent manager GameObject in the scene.
/// </summary>
public class CleaningProgressManager : MonoBehaviour
{
    public static CleaningProgressManager Instance { get; private set; }

    [Header("--- UI ---")]
    public Slider progressBar;
    public Text   progressLabel;
    public GameObject winPanel;

    // Totals are set automatically at Start() by counting spawned items
    private int _totalTasks;
    private int _completedTasks;

    // -----------------------------------------------------------------------
    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    // -----------------------------------------------------------------------
    void Start()
    {
        // Count every interactable item in the scene to get the total
        _totalTasks =
            FindObjectsByType<BookItem>(FindObjectsSortMode.None).Length +
            FindObjectsByType<TrashItem>(FindObjectsSortMode.None).Length +
            FindObjectsByType<DustItem>(FindObjectsSortMode.None).Length;

        _completedTasks = 0;

        if (winPanel != null) winPanel.SetActive(false);
        RefreshUI();

        Debug.Log($"CleaningProgressManager: {_totalTasks} tasks to complete.");
    }

    // -----------------------------------------------------------------------
    /// <summary>Call this whenever one cleaning task finishes.</summary>
    public void ReportTaskComplete()
    {
        _completedTasks = Mathf.Min(_completedTasks + 1, _totalTasks);
        RefreshUI();

        if (_completedTasks >= _totalTasks)
            TriggerWin();
    }

    // -----------------------------------------------------------------------
    void RefreshUI()
    {
        float ratio = _totalTasks > 0 ? (float)_completedTasks / _totalTasks : 0f;

        if (progressBar  != null) progressBar.value  = ratio;
        if (progressLabel != null)
            progressLabel.text = $"Clean: {_completedTasks}/{_totalTasks}";
    }

    // -----------------------------------------------------------------------
    void TriggerWin()
    {
        Debug.Log("Classroom is clean! 🎉");
        if (winPanel != null) winPanel.SetActive(true);
        Time.timeScale = 0f; // pause — remove if you prefer the game to continue
    }
}