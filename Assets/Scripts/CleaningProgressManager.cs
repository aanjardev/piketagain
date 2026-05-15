using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Tracks global cleaning progress (books shelved, trash binned, dust wiped).
/// Attach to any persistent manager GameObject in the scene.
/// </summary>
public class CleaningProgressManager : MonoBehaviour
{
    public static CleaningProgressManager Instance { get; private set; }

    [Header("--- UI ---")]
    public Slider progressBar;

    [Header("--- Text UI ---")]
    public TMP_Text progressLabel;
    public TMP_Text bookLabel;
    public TMP_Text trashLabel;
    public TMP_Text dustLabel;

    public GameObject winPanel;

    // Totals are set automatically at Start() by counting spawned items
    private int _totalBooks;
    private int _totalTrash;
    private int _totalDust;
    private int _totalTasks;

    private int _completedBooks;
    private int _completedTrash;
    private int _completedDust;
    private int _completedTasks;

    // -----------------------------------------------------------------------
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    // -----------------------------------------------------------------------
    void Start()
    {
        _totalBooks = FindObjectsByType<BookItem>(FindObjectsSortMode.None).Length;
        _totalTrash = FindObjectsByType<TrashItem>(FindObjectsSortMode.None).Length;
        _totalDust  = FindObjectsByType<DustItem>(FindObjectsSortMode.None).Length;

        _totalTasks = _totalBooks + _totalTrash + _totalDust;

        _completedBooks = 0;
        _completedTrash = 0;
        _completedDust  = 0;
        _completedTasks = 0;

        if (winPanel != null)
        {
            winPanel.SetActive(false);
        }

        RefreshUI();

        Debug.Log($"CleaningProgressManager: {_totalTasks} tasks to complete. " +
                  $"Books={_totalBooks}, Trash={_totalTrash}, Dust={_totalDust}");
    }

    // -----------------------------------------------------------------------
    /// <summary>Call this whenever one book task finishes.</summary>
    public void ReportBookComplete()
    {
        _completedBooks = Mathf.Min(_completedBooks + 1, _totalBooks);
        ReportTaskComplete();
    }

    /// <summary>Call this whenever one trash task finishes.</summary>
    public void ReportTrashComplete()
    {
        _completedTrash = Mathf.Min(_completedTrash + 1, _totalTrash);
        ReportTaskComplete();
    }

    /// <summary>Call this whenever one dust task finishes.</summary>
    public void ReportDustComplete()
    {
        _completedDust = Mathf.Min(_completedDust + 1, _totalDust);
        ReportTaskComplete();
    }

    public void ReportTaskComplete()
    {
        _completedTasks = Mathf.Min(_completedTasks + 1, _totalTasks);

        RefreshUI();

        if (_completedTasks >= _totalTasks)
        {
            TriggerWin();
        }
    }

    // -----------------------------------------------------------------------
    void RefreshUI()
    {
        float ratio = _totalTasks > 0 ? (float)_completedTasks / _totalTasks : 0f;

        if (progressBar != null)
        {
            progressBar.value = ratio;
        }

        if (progressLabel != null)
        {
            progressLabel.text = $"Clean: {_completedTasks}/{_totalTasks}";
        }

        if (bookLabel != null)
        {
            bookLabel.text = $"Books: {_completedBooks}/{_totalBooks}";
        }

        if (trashLabel != null)
        {
            trashLabel.text = $"Trash: {_completedTrash}/{_totalTrash}";
        }

        if (dustLabel != null)
        {
            dustLabel.text = $"Dust: {_completedDust}/{_totalDust}";
        }
    }

    // -----------------------------------------------------------------------
    void TriggerWin()
    {
        Debug.Log("Classroom is clean!");

        if (winPanel != null)
        {
            winPanel.SetActive(true);
        }

        Time.timeScale = 0f;
    }
}