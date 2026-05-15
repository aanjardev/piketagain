using UnityEngine;

/// <summary>
/// Attach to every Trash prefab (crumpled paper, cans, wrappers, etc.).
/// The player picks it up and throws it; landing inside the TrashCan cleans it.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class TrashItem : MonoBehaviour, IInteractable, IPickupable
{
    [Header("--- Audio ---")]
    [Tooltip("Optional: sound clip when trash is thrown into can.")]
    public AudioClip binSound;

    private Outline _outline; // Referensi untuk Quick Outline
    private bool    _wantsPickup;
    private bool    _isPickedUp;
    private bool    _isCleaned;

    // -----------------------------------------------------------------------
    void Awake()
    {
        // Otomatis mencari komponen Outline di objek sampah ini
        _outline = GetComponent<Outline>();
        if (_outline != null)
        {
            _outline.enabled = false; // Pastikan mati di awal
        }
    }

    // -----------------------------------------------------------------------
    #region IInteractable

    public void OnLookAt()
    {
        if (_isPickedUp || _isCleaned) return;
        
        // Nyalakan outline saat dilihat
        if (_outline != null) _outline.enabled = true;
    }

    public void OnLookAway()
    {
        // Matikan outline saat kursor pergi
        if (_outline != null) _outline.enabled = false;
    }

    public string GetPromptText() => "[E] Pick up Trash";

    public void Interact(GameObject player)
    {
        if (_isPickedUp || _isCleaned) return;
        _wantsPickup = true;
        _isPickedUp  = true;
        OnLookAway(); // Matikan highlight saat dipungut
    }

    #endregion

    // -----------------------------------------------------------------------
    #region IPickupable

    public bool WantsPickup()
    {
        if (_wantsPickup)
        {
            _wantsPickup = false;
            return true;
        }
        return false;
    }

    #endregion

    // -----------------------------------------------------------------------
    /// <summary>
    /// Called by TrashCan's trigger when this item enters it after being thrown.
    /// </summary>
    public void OnDisposedInCan()
    {
        if (_isCleaned) return;
        _isCleaned = true;

        if (binSound != null)
            AudioSource.PlayClipAtPoint(binSound, transform.position);

        Debug.Log($"{name} disposed.");
        CleaningProgressManager.Instance?.ReportTrashComplete();
        Destroy(gameObject, 0.05f); // jeda sedikit agar suara sempat diputar sebelum objek hancur
    }
}