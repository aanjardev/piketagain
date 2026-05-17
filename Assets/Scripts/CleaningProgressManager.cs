using System.Collections;
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

    // Read-only accessors for other systems/UI
    public int TotalBooks => _totalBooks;
    public int TotalTrash => _totalTrash;
    public int TotalDust => _totalDust;
    public int TotalDesks => _totalDesks;
    public int TotalChairs => _totalChairs;

    public int CompletedBooks => _completedBooks;
    public int CompletedTrash => _completedTrash;
    public int CompletedDust => _completedDust;
    public int CompletedDesks => _completedDesks;
    public int CompletedChairs => _completedChairs;

    [Header("--- UI ---")]
    public Slider progressBar;

    [Header("--- Text UI ---")]
    public TMP_Text progressLabel;
    public TMP_Text bookLabel;
    public TMP_Text trashLabel;
    public TMP_Text dustLabel;
    public TMP_Text deskLabel;
    public TMP_Text chairLabel;

    public GameObject winPanel;

    // Totals are set automatically at Start() by counting spawned items
    private int _totalBooks;
    private int _totalTrash;
    private int _totalDust;
    private int _totalDesks;
    private int _totalChairs;
    private int _totalTasks;

    private int _completedBooks;
    private int _completedTrash;
    private int _completedDust;
    private int _completedDesks;
    private int _completedChairs;
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
    IEnumerator Start()
    {
        // Wait until all spawners/Start() methods have run.
        yield return new WaitForEndOfFrame();

        BookItem[] books = FindObjectsByType<BookItem>(FindObjectsSortMode.None);
        _totalBooks = 0;
        foreach (var book in books)
        {
            if (!book.IsShelved)
                _totalBooks++;
        }

        _totalTrash = CountItemsWithFallback<TrashItem>("Trash");
        _totalDust  = CountItemsWithFallback<DustItem>("Dust");

        DeskMessy[] desks = FindObjectsByType<DeskMessy>(FindObjectsSortMode.None);
        _totalDesks = 0;
        foreach (var desk in desks)
        {
            if (desk.IsMessy)
                _totalDesks++;
        }

        ChairMessy[] chairs = FindObjectsByType<ChairMessy>(FindObjectsSortMode.None);
        _totalChairs = 0;
        foreach (var chair in chairs)
        {
            if (chair.IsMessy)
                _totalChairs++;
        }

        _totalTasks = _totalBooks + _totalTrash + _totalDust + _totalDesks + _totalChairs;

        _completedBooks = 0;
        _completedTrash = 0;
        _completedDust  = 0;
        _completedDesks = 0;
        _completedChairs = 0;
        _completedTasks = 0;

        if (winPanel != null)
        {
            winPanel.SetActive(false);
        }

        RefreshUI();

        Debug.Log($"CleaningProgressManager: {_totalTasks} tasks to complete. " +
                  $"Books={_totalBooks}, Trash={_totalTrash}, Dust={_totalDust}, Desks={_totalDesks}, Chairs={_totalChairs}");
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

    public void ReportDeskComplete()
    {
        _completedDesks = Mathf.Min(_completedDesks + 1, _totalDesks);
        ReportTaskComplete();
    }

    public void ReportChairComplete()
    {
        _completedChairs = Mathf.Min(_completedChairs + 1, _totalChairs);
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

        if (deskLabel != null)
        {
            deskLabel.text = $"Desks: {_completedDesks}/{_totalDesks}";
        }

        if (chairLabel != null)
        {
            chairLabel.text = $"Chairs: {_completedChairs}/{_totalChairs}";
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

    int CountItemsWithFallback<T>(string fallbackTag) where T : MonoBehaviour
    {
        T[] items = FindObjectsByType<T>(FindObjectsSortMode.None);
        if (items.Length == 0)
        {
            items = GameObject.FindObjectsByType<T>(FindObjectsSortMode.None);
            if (items.Length > 0 && !string.IsNullOrEmpty(fallbackTag))
                Debug.Log($"[CleaningProgressManager] Found {items.Length} {typeof(T).Name} via FindObjectsOfType fallback.");
        }

        if (items.Length > 0)
            return items.Length;

        if (!string.IsNullOrEmpty(fallbackTag))
        {
            GameObject[] tagged = GameObject.FindGameObjectsWithTag(fallbackTag);
            if (tagged.Length > 0)
            {
                Debug.Log($"[CleaningProgressManager] Found {tagged.Length} objects tagged '{fallbackTag}' as fallback.");
                return tagged.Length;
            }
        }

        return 0;
    }
}