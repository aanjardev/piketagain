using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HintLocator : MonoBehaviour
{
    [Header("--- References ---")]
    public Transform player;
    public Camera playerCamera;

    [Header("--- Hint Marker ---")]
    public GameObject hintMarkerPrefab;
    public float markerHeightOffset = 0.8f;
    public float markerDuration = 4f;

    [Header("--- Hint Limit ---")]
    public int maxHintUses = 3;
    private int _hintUsed = 0;

    [Header("--- Controls ---")]
    public KeyCode hintKey = KeyCode.H;

    [Header("--- Optional UI ---")]
    public CanvasGroup hintPanel;
    public TMP_Text hintText;

    private GameObject _currentMarker;
    private GameObject _lastTarget;
    private Coroutine _hideRoutine;

    void Awake()
    {
        if (playerCamera == null)
            playerCamera = Camera.main;
    }

    void Update()
    {
        if (Input.GetKeyDown(hintKey))
        {
            ShowNextHint();
        }
    }

    public void ShowNextHint()
    {
        if (_hintUsed >= maxHintUses)
        {
            ShowHintText("Hint sudah habis.");
            ClearMarker();

            if (_hideRoutine != null)
                StopCoroutine(_hideRoutine);

            _hideRoutine = StartCoroutine(HideTextAfterDelay());
            return;
        }

        List<GameObject> unfinishedTargets = GetUnfinishedTargets();

        if (unfinishedTargets.Count == 0)
        {
            ShowHintText("Semua area sudah bersih!");
            ClearMarker();

            if (_hideRoutine != null)
                StopCoroutine(_hideRoutine);

            _hideRoutine = StartCoroutine(HideTextAfterDelay());
            return;
        }

        GameObject target = GetNextTarget(unfinishedTargets);

        if (target == null)
        {
            ShowHintText("Belum ada target hint.");
            ClearMarker();
            return;
        }

        _hintUsed++;

        _lastTarget = target;

        ShowMarkerOnTarget(target);

        int remainingHint = maxHintUses - _hintUsed;
        ShowHintText(GetTargetMessage(target) + $"\nSisa hint: {remainingHint}");

        if (_hideRoutine != null)
            StopCoroutine(_hideRoutine);

        _hideRoutine = StartCoroutine(HideMarkerAfterDelay());
    }

    List<GameObject> GetUnfinishedTargets()
    {
        List<GameObject> targets = new List<GameObject>();

        AddActiveObjectsWithTag(targets, "Trash");
        AddActiveObjectsWithTag(targets, "Dust");
        AddUnfinishedBooks(targets);

        return targets;
    }

    void AddActiveObjectsWithTag(List<GameObject> targets, string tagName)
    {
        GameObject[] objects = SafeFindGameObjectsWithTag(tagName);

        foreach (GameObject obj in objects)
        {
            if (obj == null) continue;
            if (!obj.activeInHierarchy) continue;

            targets.Add(obj);
        }
    }

    void AddUnfinishedBooks(List<GameObject> targets)
    {
        GameObject[] books = SafeFindGameObjectsWithTag("Book");

        foreach (GameObject obj in books)
        {
            if (obj == null) continue;
            if (!obj.activeInHierarchy) continue;

            BookItem book = obj.GetComponent<BookItem>();

            if (book != null && book.IsShelved)
                continue;

            targets.Add(obj);
        }
    }

    GameObject[] SafeFindGameObjectsWithTag(string tagName)
    {
        try
        {
            return GameObject.FindGameObjectsWithTag(tagName);
        }
        catch
        {
            Debug.LogWarning($"Tag '{tagName}' belum dibuat di Unity.");
            return new GameObject[0];
        }
    }

    GameObject GetNextTarget(List<GameObject> targets)
    {
        SortTargetsByDistance(targets);

        if (_lastTarget == null || !targets.Contains(_lastTarget))
            return targets[0];

        int currentIndex = targets.IndexOf(_lastTarget);
        int nextIndex = (currentIndex + 1) % targets.Count;

        return targets[nextIndex];
    }

    void SortTargetsByDistance(List<GameObject> targets)
    {
        if (player == null) return;

        targets.Sort((a, b) =>
        {
            float distA = Vector3.Distance(player.position, a.transform.position);
            float distB = Vector3.Distance(player.position, b.transform.position);
            return distA.CompareTo(distB);
        });
    }

    void ShowMarkerOnTarget(GameObject target)
    {
        ClearMarker();

        if (hintMarkerPrefab == null)
        {
            Debug.LogWarning("Hint Marker Prefab belum diisi di Inspector.");
            return;
        }

        Vector3 markerPos = GetMarkerPosition(target);

        _currentMarker = Instantiate(hintMarkerPrefab, markerPos, Quaternion.identity);
        _currentMarker.name = $"HintMarker_{target.name}";
    }

    Vector3 GetMarkerPosition(GameObject target)
    {
        Renderer renderer = target.GetComponentInChildren<Renderer>();

        if (renderer != null)
            return renderer.bounds.center + Vector3.up * markerHeightOffset;

        Collider collider = target.GetComponentInChildren<Collider>();

        if (collider != null)
            return collider.bounds.center + Vector3.up * markerHeightOffset;

        return target.transform.position + Vector3.up * markerHeightOffset;
    }

    string GetTargetMessage(GameObject target)
    {
        if (target.CompareTag("Trash"))
            return "Hint: Masih ada sampah di area ini.";

        if (target.CompareTag("Dust"))
            return "Hint: Masih ada debu/noda di area ini.";

        if (target.CompareTag("Book"))
            return "Hint: Masih ada buku yang belum ditaruh ke rak.";

        return "Hint: Ada tugas yang belum selesai di area ini.";
    }

    void ShowHintText(string message)
    {
        if (hintText != null)
            hintText.text = message;

        if (hintPanel != null)
        {
            hintPanel.alpha = 1f;
            hintPanel.interactable = false;
            hintPanel.blocksRaycasts = false;
        }
    }

    IEnumerator HideMarkerAfterDelay()
    {
        yield return new WaitForSeconds(markerDuration);

        ClearMarker();

        if (hintPanel != null)
            hintPanel.alpha = 0f;

        _hideRoutine = null;
    }

    IEnumerator HideTextAfterDelay()
    {
        yield return new WaitForSeconds(2f);

        if (hintPanel != null)
            hintPanel.alpha = 0f;

        _hideRoutine = null;
    }

    void ClearMarker()
    {
        if (_currentMarker != null)
        {
            Destroy(_currentMarker);
            _currentMarker = null;
        }
    }
}