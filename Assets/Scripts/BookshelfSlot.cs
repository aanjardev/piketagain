using UnityEngine;

// =============================================================================
//  BOOKSHELF SLOT
// =============================================================================

public class BookshelfSlot : MonoBehaviour, IInteractable
{
    [Tooltip("Maksimal buku yang bisa ditaruh di slot ini.")]
    public int capacity = 1;

    private int _booksPlaced;
    private Outline _outline;

    public bool IsFull => _booksPlaced >= capacity;

    void Awake()
    {
        // Menambahkan dukungan Quick Outline agar slotnya menyala saat disorot
        _outline = GetComponent<Outline>();
        if (_outline != null) _outline.enabled = false;
    }

    // -----------------------------------------------------------------------
    #region IInteractable

    public void OnLookAt()
    {
        if (!IsFull && _outline != null) 
            _outline.enabled = true;
    }

    public void OnLookAway() 
    { 
        if (_outline != null) 
            _outline.enabled = false; 
    }

    public string GetPromptText()
    {
        if (IsFull) return "Rak Penuh";

        // Cek apakah pemain sedang memegang buku
        PlayerInteraction pi = FindAnyObjectByType<PlayerInteraction>();
        if (pi != null && pi.heldItem != null && pi.heldItem.CompareTag("Book"))
        {
            return "[E] Taruh Buku";
        }
        
        return "Butuh Buku"; // Pesan jika pemain menyorot rak tanpa memegang buku
    }

    public void Interact(GameObject player)
    {
        if (IsFull) return;

        if (player.TryGetComponent(out PlayerInteraction pi))
        {
            // Pastikan pemain benar-benar memegang buku sebelum memproses
            if (pi.heldItem != null && pi.heldItem.CompareTag("Book"))
            {
                _booksPlaced++;

                // Mainkan shelve sound saat menaruh buku
                AudioLibrary.Instance?.PlayBookShelve(transform.position);

                // Panggil fungsi OnShelved di buku (jika ada script BookItem)
                if (pi.heldItem.TryGetComponent(out BookItem book))
                {
                    // Pastikan di dalam script BookItem kamu ada fungsi public void OnShelved()
                    book.OnShelved(); 
                }

                Debug.Log($"Buku ditaruh di {name}! Total: {_booksPlaced}/{capacity}");
                OnLookAway(); // Matikan highlight kuning setelah ditaruh
            }
        }
    }

    #endregion
}