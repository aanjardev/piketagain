using UnityEngine;
using TMPro;
using System.Collections;

public class ProgressListController : MonoBehaviour
{
    public static ProgressListController Instance { get; private set; }

    [Header("--- UI References ---")]
    public RectTransform panelRect;
    public CanvasGroup panelGroup;
    public TMP_Text titleText;
    public TMP_Text tooltipText;

    [Header("--- Task Texts ---")]
    public TMP_Text task1; // Rapikan meja
    public TMP_Text task2; // Rapikan kursi
    public TMP_Text task3; // Buang sampah
    public TMP_Text task4; // Rapikan buku
    public TMP_Text task5; // Bersihkan noda

    [Header("--- Task Checkmarks ---")]
    public GameObject task1Check;
    public GameObject task2Check;
    public GameObject task3Check;
    public GameObject task4Check;
    public GameObject task5Check;

    [Header("--- Slide Settings ---")]
    [Tooltip("Local Y offset applied when collapsed. Use negative value to move panel down off-screen leaving a small peek.")]
    public float collapsedOffsetY = -180f;
    public float animationDuration = 0.25f;
    [Tooltip("Optional: RectTransform used as target position when expanded (e.g. center anchor). If null, current Rect's anchoredPosition is used.")]
    public RectTransform expandedTarget;
    [Tooltip("Optional: RectTransform used as target position when collapsed (e.g. bottom-left peek). If null, collapsed position is computed from collapsedOffsetY.")]
    public RectTransform collapsedTarget;
    [Tooltip("Enable debug log for progress list open/collapse behavior.")]
    public bool logProgressList = false;

    RectTransform _rect;
    Vector2 _expandedPos;
    Vector2 _collapsedPos;
    Coroutine _anim;

    void Awake()
    {
        Instance = this;

        _rect = panelRect != null ? panelRect : GetComponent<RectTransform>();
        if (_rect != null)
        {
            // Determine expanded position
            if (expandedTarget != null)
                _expandedPos = GetLocalAnchoredPosFrom(expandedTarget);
            else
                _expandedPos = _rect.anchoredPosition;

            // Determine collapsed position
            if (collapsedTarget != null)
                _collapsedPos = GetLocalAnchoredPosFrom(collapsedTarget);
            else
                _collapsedPos = _expandedPos + new Vector2(0f, collapsedOffsetY);

            // Start collapsed so only top edge shows
            _rect.anchoredPosition = _collapsedPos;
        }

        if (panelGroup != null)
        {
            panelGroup.alpha = 1f; // keep visible (we slide it)
            panelGroup.blocksRaycasts = false;
            panelGroup.interactable = false;
        }

        // hide all checklist icons at startup
        if (task1Check != null) task1Check.SetActive(false);
        if (task2Check != null) task2Check.SetActive(false);
        if (task3Check != null) task3Check.SetActive(false);
        if (task4Check != null) task4Check.SetActive(false);
        if (task5Check != null) task5Check.SetActive(false);

        // initialize tooltip
        if (tooltipText != null) tooltipText.text = "Press 'tab' to open";
    }

    void Start()
    {
        // Ensure the panel is collapsed on first frame after Awake
        if (logProgressList)
            Debug.Log($"[ProgressListController] Start collapsedPos={_collapsedPos} expandedPos={_expandedPos}");
        SetOpen(false);
    }

    void Update()
    {
        RefreshUI();
    }

    public void SetOpen(bool open)
    {
        if (_rect == null) return;

        if (logProgressList)
            Debug.Log($"[ProgressListController] SetOpen({open}) targetCollapsed={_collapsedPos} targetExpanded={_expandedPos} current={_rect.anchoredPosition}");
        if (_rect == null) return;

        // animate to target position
        Vector2 target = open ? _expandedPos : _collapsedPos;
        if (_anim != null) StopCoroutine(_anim);
        _anim = StartCoroutine(AnimatePosition(_rect, target, animationDuration));

        if (tooltipText != null)
            tooltipText.text = open ? "Press 'tab' to close" : "Press 'tab' to open";

        if (panelGroup != null)
        {
            panelGroup.blocksRaycasts = open;
            panelGroup.interactable = open;
        }
    }

    IEnumerator AnimatePosition(RectTransform r, Vector2 target, float duration)
    {
        Vector2 start = r.anchoredPosition;
        float t = 0f;
        while (t < 1f)
        {
            t += Time.unscaledDeltaTime / Mathf.Max(0.0001f, duration);
            r.anchoredPosition = Vector2.Lerp(start, target, Mathf.SmoothStep(0f,1f,t));
            yield return null;
        }
        r.anchoredPosition = target;
        _anim = null;
    }

    // Helper: convert a target RectTransform anchoredPosition into this panel's local anchored space
    Vector2 GetLocalAnchoredPosFrom(RectTransform target)
    {
        if (target == null || _rect == null) return Vector2.zero;

        // If the anchors share the same parent, use anchoredPosition directly
        if (target.parent == _rect.parent)
            return target.anchoredPosition;

        Vector3 worldPos = target.position;
        RectTransform parentRect = _rect.parent as RectTransform;
        if (parentRect == null)
            return target.anchoredPosition;

        // Use the correct camera for the Canvas render mode
        Canvas canvas = target.GetComponentInParent<Canvas>();
        Camera cam = canvas != null && canvas.renderMode != RenderMode.ScreenSpaceOverlay ? canvas.worldCamera : null;

        Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(cam, worldPos);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRect, screenPoint, cam, out Vector2 localPos);
        return localPos;
    }

    public void RefreshUI()
    {
        var mgr = CleaningProgressManager.Instance;
        if (mgr == null) return;

        bool deskDone = mgr.TotalDesks > 0 && mgr.CompletedDesks >= mgr.TotalDesks;
        bool chairDone = mgr.TotalChairs > 0 && mgr.CompletedChairs >= mgr.TotalChairs;
        bool trashDone = mgr.TotalTrash > 0 && mgr.CompletedTrash >= mgr.TotalTrash;
        bool bookDone = mgr.TotalBooks > 0 && mgr.CompletedBooks >= mgr.TotalBooks;
        bool dustDone = mgr.TotalDust > 0 && mgr.CompletedDust >= mgr.TotalDust;

        if (task1 != null)
            task1.text = $"Rapikan meja ({mgr.CompletedDesks}/{mgr.TotalDesks})";
        if (task1Check != null)
            task1Check.SetActive(deskDone);

        if (task2 != null)
            task2.text = $"Rapikan kursi ({mgr.CompletedChairs}/{mgr.TotalChairs})";
        if (task2Check != null)
            task2Check.SetActive(chairDone);

        if (task3 != null)
            task3.text = $"Buang sampah ({mgr.CompletedTrash}/{mgr.TotalTrash})";
        if (task3Check != null)
            task3Check.SetActive(trashDone);

        if (task4 != null)
            task4.text = $"Rapikan buku ({mgr.CompletedBooks}/{mgr.TotalBooks})";
        if (task4Check != null)
            task4Check.SetActive(bookDone);

        if (task5 != null)
            task5.text = $"Bersihkan noda ({mgr.CompletedDust}/{mgr.TotalDust})";
        if (task5Check != null)
            task5Check.SetActive(dustDone);
    }
}
