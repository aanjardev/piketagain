using UnityEngine;

[RequireComponent(typeof(Collider))]
public class TrashItem : MonoBehaviour, IInteractable
{
    private Outline _outline;
    private bool _isCleaned = false;

    void Awake()
    {
        _outline = GetComponent<Outline>();
        if (_outline != null) _outline.enabled = false;
    }

    public void OnLookAt()
    {
        if (_outline != null) _outline.enabled = true;
    }

    public void OnLookAway()
    {
        if (_outline != null) _outline.enabled = false;
    }

    public string GetPromptText()
    {
        return "[E] Masukkan ke Kantong";
    }

    public void Interact(GameObject player)
    {
        PlayerInteraction pi = player.GetComponent<PlayerInteraction>();

        if (pi == null) return;

        // Cek apakah pemain sedang memakai Kantong Sampah
        if (pi.currentTool == ToolType.KantongSampah)
        {
            if (pi.isiKantongSaatIni < pi.kapasitasKantong)
            {
                pi.isiKantongSaatIni++;

                AudioLibrary.Instance?.PlayTrashPickup(transform.position);

                // Trash dihitung selesai saat masuk kantong
                CleaningProgressManager.Instance?.ReportTrashComplete();

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

    // Opsional: dipakai kalau suatu saat sampah bisa langsung masuk tong lewat trigger.
    // Karena progress sudah dihitung saat masuk kantong, jangan ReportTrashComplete() di sini.
    public void OnDisposedInCan()
    {
        if (_isCleaned) return;

        _isCleaned = true;

        AudioLibrary.Instance?.PlayTrashDispose(transform.position);

        Debug.Log($"{name} disposed.");

        Destroy(gameObject, 0.05f);
    }
}