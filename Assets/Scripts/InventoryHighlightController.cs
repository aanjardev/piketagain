using UnityEngine;
using UnityEngine.UI;

public class InventoryHighlightController : MonoBehaviour
{
    [Header("--- Highlight Images ---")]
    public Image highlight1;
    public Image highlight2;
    public Image highlight3;

    [Header("--- Alpha Values ---")]
    [Tooltip("Alpha untuk highlight yang aktif")]
    public float activeAlpha = 216f / 255f; // 216/255 ≈ 0.847
    [Tooltip("Alpha untuk highlight yang tidak aktif")]
    public float inactiveAlpha = 136f / 255f; // 136/255 ≈ 0.533

    private int _currentHighlight = 1; // Highlight 1 aktif di awal

    void Awake()
    {
        // Ensure all highlights are initialized
        UpdateAllHighlights();
    }

    void Update()
    {
        HandleHighlightInput();
    }

    void HandleHighlightInput()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
            SelectHighlight(1);
        else if (Input.GetKeyDown(KeyCode.Alpha2))
            SelectHighlight(2);
        else if (Input.GetKeyDown(KeyCode.Alpha3))
            SelectHighlight(3);
    }

    public void SelectHighlight(int highlightNumber)
    {
        if (highlightNumber < 1 || highlightNumber > 3) return;

        _currentHighlight = highlightNumber;
        UpdateAllHighlights();

        Debug.Log($"[InventoryHighlight] Selected highlight {highlightNumber}");
    }

    void UpdateAllHighlights()
    {
        UpdateHighlightAlpha(highlight1, _currentHighlight == 1);
        UpdateHighlightAlpha(highlight2, _currentHighlight == 2);
        UpdateHighlightAlpha(highlight3, _currentHighlight == 3);
    }

    void UpdateHighlightAlpha(Image highlight, bool isActive)
    {
        if (highlight == null) return;

        Color color = highlight.color;
        color.a = isActive ? activeAlpha : inactiveAlpha;
        highlight.color = color;
    }
}
