using UnityEngine;

// Tambahkan IInteractable agar tong sampah bisa disorot dan diklik pemain
public class TrashCan : MonoBehaviour, IInteractable
{
    [Header("--- Visual ---")]
    [Tooltip("Particle effect played when trash lands inside.")]
    public ParticleSystem binParticles;

    private int _trashCount;
    private Outline _outline; // Untuk efek menyala Quick Outline

    public int TrashCount => _trashCount;

    // -----------------------------------------------------------------------
    void Awake()
    {
        // Mencari komponen Outline otomatis
        _outline = GetComponent<Outline>();
        if (_outline != null) _outline.enabled = false;
    }

    // -----------------------------------------------------------------------
    #region IInteractable

    public void OnLookAt()
    {
        // Nyalakan outline saat pemain melihat ke tong sampah
        if (_outline != null) _outline.enabled = true;
    }

    public void OnLookAway()
    {
        // Matikan outline saat berpaling
        if (_outline != null) _outline.enabled = false;
    }

    public string GetPromptText() => "[E] Buang Sampah";

    public void Interact(GameObject player)
    {
        // Dikosongkan karena aksi membuangnya diatur oleh script PlayerInteraction
    }

    #endregion

    // -----------------------------------------------------------------------
    // Fungsi ini akan dipanggil oleh PlayerInteraction saat menekan [E]
    public void ReceiveTrash(TrashItem trash)
    {
        _trashCount++;
        trash.OnDisposedInCan();

        if (binParticles != null)
            binParticles.Play();
            
        Debug.Log($"Sampah berhasil dibuang! Total: {_trashCount}");
    }
}