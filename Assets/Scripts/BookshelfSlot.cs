using UnityEngine;

// =============================================================================
//  BOOKSHELF SLOT
// =============================================================================

/// <summary>
/// Attach one BookshelfSlot per book slot on your Bookshelf model.
/// Each slot is an empty child GameObject positioned where a book should sit.
/// </summary>
public class BookshelfSlot : MonoBehaviour, IInteractable
{
    [Tooltip("Maximum number of books this slot can hold.")]
    public int capacity = 1;

    private int _booksPlaced;

    // -----------------------------------------------------------------------
    public bool IsFull => _booksPlaced >= capacity;

    // -----------------------------------------------------------------------
    #region IInteractable

    public void OnLookAt()
    {
        // Optionally glow the slot to guide the player
        Debug.Log($"BookshelfSlot '{name}': {_booksPlaced}/{capacity} books.");
    }

    public void OnLookAway() { }

    public string GetPromptText() =>
        IsFull ? "Shelf full" : "[E] Place Book";

    public void Interact(GameObject player)
    {
        // The actual placement is handled in PlayerInteraction.TryInteract()
        // This is called right before PlaceHeldItemAt().
        if (IsFull) return;

        _booksPlaced++;

        // Notify the BookItem
        // (PlayerInteraction passes the held object's root — look for BookItem there)
        if (player.TryGetComponent(out PlayerInteraction pi))
        {
            // Access held item via reflection isn't ideal; use an event instead.
            // For simplicity we find it via the holdPoint child.
            Transform holdPoint = pi.holdPoint;
            if (holdPoint.childCount > 0)
            {
                GameObject heldObj = holdPoint.GetChild(0).gameObject;
                if (heldObj.TryGetComponent(out BookItem book))
                    book.OnShelved();
            }
        }
    }

    #endregion
}

