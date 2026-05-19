using UnityEngine;

[RequireComponent(typeof(Collider))]
public class TrashItem : MonoBehaviour, IInteractable
{
    [Header("--- Audio ---")]
    [Tooltip("Suara saat sampah dimasukkan ke kantong.")]
    public AudioClip pickupSound;

    [Tooltip("Suara saat sampah masuk ke tong. Opsional.")]
    public AudioClip binSound;

    private Outline _outline;
    private bool _isCleaned = false;
    private PlayerInteraction _player;

    void Awake()
    {
        _outline = GetComponent<Outline>();
        if (_outline != null) _outline.enabled = false;
        _player = FindAnyObjectByType<PlayerInteraction>();
    }

    public void OnLookAt()
    {
        if (_isCleaned) return;
        if (_outline != null) _outline.enabled = true;
    }

    public void OnLookAway()
    {
        if (_outline != null) _outline.enabled = false;
    }

    public string GetPromptText()
    {
        if (_isCleaned) return "";

        if (_player == null)
            _player = FindAnyObjectByType<PlayerInteraction>();

        if (_player == null)
            return "[E] Masukkan ke Kantong";

        if (_player.currentTool != ToolType.KantongSampah)
            return "Butuh Kantong Sampah (Tekan 2)";

        if (_player.isiKantongSaatIni >= _player.kapasitasKantong)
            return "Kantong penuh! Buang dulu ke tong";

        return "[E] Masukkan ke Kantong";
    }

    public void Interact(GameObject player)
    {
        if (_isCleaned) return;

        PlayerInteraction pi = player.GetComponent<PlayerInteraction>();

        if (pi == null)
            return;

        _player = pi;

        if (pi.currentTool != ToolType.KantongSampah)
        {
            Debug.Log("Butuh Kantong Sampah! (Tekan 2)");
            return;
        }

        if (pi.isiKantongSaatIni >= pi.kapasitasKantong)
        {
            Debug.Log("Kantong Penuh! Buang dulu ke tong utama.");
            return;
        }

        pi.isiKantongSaatIni++;

        if (pickupSound != null)
            SoundManager.Instance?.PlaySFXAtPoint(pickupSound, transform.position, 1f);

        _isCleaned = true;

        if (_outline != null)
            _outline.enabled = false;

        CleaningProgressManager.Instance?.ReportTrashComplete();

        Destroy(gameObject);
    }

    // Opsional: dipakai kalau suatu saat sampah bisa langsung masuk tong lewat trigger.
    // Karena progress sudah dihitung saat masuk kantong, jangan ReportTrashComplete() di sini.
    public void OnDisposedInCan()
    {
        if (_isCleaned) return;

        _isCleaned = true;

        if (_outline != null)
            _outline.enabled = false;

        if (binSound != null)
            SoundManager.Instance?.PlaySFXAtPoint(binSound, transform.position, 1f);

        Debug.Log($"{name} disposed.");

        Destroy(gameObject, 0.05f);
    }
}