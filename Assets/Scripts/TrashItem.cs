using UnityEngine;

[RequireComponent(typeof(Collider))]
public class TrashItem : MonoBehaviour, IInteractable
{
    [Header("--- Audio ---")]
    [Tooltip("Suara saat sampah dimasukkan ke kantong.")]
    public AudioClip pickupSound;

    private Outline _outline;

    void Awake()
    {
        _outline = GetComponent<Outline>();
        if (_outline != null) _outline.enabled = false;
    }

    public void OnLookAt() { if (_outline != null) _outline.enabled = true; }
    public void OnLookAway() { if (_outline != null) _outline.enabled = false; }

    public string GetPromptText() => "[E] Masukkan ke Kantong";

    public void Interact(GameObject player)
    {
        PlayerInteraction pi = player.GetComponent<PlayerInteraction>();

        // Cek apakah pemain sedang memakai Kantong Sampah
        if (pi.currentTool == ToolType.KantongSampah)
        {
            if (pi.isiKantongSaatIni < pi.kapasitasKantong)
            {
                pi.isiKantongSaatIni++;
                
                if (pickupSound != null)
                    AudioSource.PlayClipAtPoint(pickupSound, transform.position);
                    
                Destroy(gameObject); // Hilangkan sampah
            }
            else
            {
                Debug.Log("Kantong Penuh! Buang dulu ke tong utama.");
            }
        }
        else
        {
            Debug.Log("Butuh Kantong Sampah! (Tekan 2)");
        }
    }
}