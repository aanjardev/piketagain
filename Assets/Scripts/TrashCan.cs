using UnityEngine;

public class TrashCan : MonoBehaviour, IInteractable
{
    [Header("--- Visual & Animation ---")]
    public Animator animator; 
    public ParticleSystem binParticles;
    public AudioClip emptyBagSound;

    private int _totalSampahDiTong;
    private Outline _outline;
    
    // Status ini bisa dibaca oleh script pemain
    public bool IsOpen { get; private set; } 

    void Awake()
    {
        _outline = GetComponent<Outline>();
        if (_outline != null) _outline.enabled = false;
    }

    public void OnLookAt() { if (_outline != null) _outline.enabled = true; }
    public void OnLookAway() { if (_outline != null) _outline.enabled = false; }

    public string GetPromptText() => IsOpen ? "[E] Tutup Tong" : "[E] Buka Tong";

    public void Interact(GameObject player)
    {
        // Tombol E murni hanya untuk animasi buka/tutup tong
        IsOpen = !IsOpen;

        if (animator != null)
        {
            animator.SetBool("IsOpen", IsOpen);
        }
    }

    // Fungsi ini dipanggil oleh PlayerInteraction saat selesai menahan [Q]
    public void EmptyBagIntoCan(int amount)
    {
        _totalSampahDiTong += amount;
        
        if (binParticles != null) binParticles.Play();
        if (emptyBagSound != null) SoundManager.Instance?.PlaySFXAtPoint(emptyBagSound, transform.position, 1f);

        Debug.Log($"Berhasil membuang {amount} sampah! Total di tong: {_totalSampahDiTong}");
    }
}