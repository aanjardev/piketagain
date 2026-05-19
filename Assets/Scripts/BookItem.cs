using UnityEngine;

/// <summary>
/// Attach to every Book prefab.
/// The player picks the book up; it is then placed into a BookshelfSlot.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class BookItem : MonoBehaviour, IInteractable, IPickupable
{
    private Outline _outline; // Referensi untuk Quick Outline
    private bool    _wantsPickup;
    private bool    _isPickedUp;
    private bool    _isShelved;

    public bool IsShelved => _isShelved;

    // -----------------------------------------------------------------------
    void Awake()
    {
        // Otomatis mencari komponen Outline di objek buku ini
        _outline = GetComponent<Outline>();
        if (_outline != null)
        {
            _outline.enabled = false; // Pastikan mati di awal
        }

        // Books start physics-enabled so they settle on desks naturally
        GetComponent<Rigidbody>().collisionDetectionMode = CollisionDetectionMode.Continuous;
    }

    // -----------------------------------------------------------------------
    #region IInteractable

    public void OnLookAt()
    {
        if (_isPickedUp) return;
        
        // Nyalakan outline saat dilihat
        if (_outline != null) _outline.enabled = true;
    }

    public void OnLookAway()
    {
        // Matikan outline saat berpaling
        if (_outline != null) _outline.enabled = false;
    }

    public string GetPromptText() => "[E] Pick up Book";

    public void Interact(GameObject player)
    {
        if (_isPickedUp) return;
        _wantsPickup = true;
        _isPickedUp  = true;
        OnLookAway(); // Hilangkan highlight

        // Mainkan pickup sound saat mengangkat buku
        AudioLibrary.Instance?.PlayBookPickup(transform.position);
    }

    #endregion

    // -----------------------------------------------------------------------
    #region IPickupable

    public bool WantsPickup()
    {
        if (_wantsPickup)
        {
            _wantsPickup = false; // consume the flag
            return true;
        }
        return false;
    }

    #endregion

    public void OnDropped()
    {
        if (_isShelved) return;

        _isPickedUp = false;
        _wantsPickup = false;

        if (_outline != null)
            _outline.enabled = false;

        Debug.Log($"{name} dropped.");
    }
    // -----------------------------------------------------------------------
    public void OnShelved()
    {
        _isPickedUp = false;
        _isShelved = true;
        var rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity  = false;
        Debug.Log($"{name} shelved.");

        CleaningProgressManager.Instance?.ReportBookComplete();
    }
}