using UnityEngine;

/// <summary>
/// Buku kuning khusus untuk quest etika.
/// Player bisa mengambilnya, dan kemudian mengembalikannya ke NPC.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class YellowBookItem : MonoBehaviour, IInteractable, IPickupable
{
    private Outline _outline; // Referensi untuk Quick Outline
    private bool _wantsPickup;
    private bool _isPickedUp;

    // -----------------------------------------------------------------------
    void Awake()
    {
        // Otomatis mencari komponen Outline di objek buku ini
        _outline = GetComponent<Outline>();
        if (_outline != null)
        {
            _outline.enabled = false; // Pastikan mati di awal
        }

        // Yellow book starts with physics enabled so it can settle naturally
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

    public string GetPromptText() => "[E] Ambil Buku Kuning";

    public void Interact(GameObject player)
    {
        if (_isPickedUp) return;
        _wantsPickup = true;
        _isPickedUp = true;
        OnLookAway(); // Hilangkan highlight
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

    // -----------------------------------------------------------------------
    /// <summary>Dipanggil saat player sudah mengambil buku (pas di hold point)</summary>
    public void OnPickedUp()
    {
        // Notify sistem bahwa player sudah punya buku
        GameSessionManager.PickUpYellowBook();
        Debug.Log("Yellow Book picked up!");
    }

    /// <summary>Dipanggil saat player mengembalikan buku ke NPC</summary>
    public void OnReturned()
    {
        Debug.Log("Yellow Book returned to NPC!");
    }
}
