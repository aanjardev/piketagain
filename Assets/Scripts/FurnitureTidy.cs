using System.Collections;
using UnityEngine;

public class FurnitureTidy : MonoBehaviour, IInteractable
{
    [Header("--- Tidy Settings ---")]
    [Tooltip("Kosongkan jika hanya ingin menegakkan kursi. Isi dengan objek kosong (titik target) jika ingin kursi pindah ke posisi spesifik.")]
    public Transform neatTarget;
    
    [Tooltip("Berapa detik waktu animasi kursinya bergeser/merapikan diri?")]
    public float tidyDuration = 0.4f;
    
    [Header("--- Audio (Opsional) ---")]
    public AudioClip slideSound;

    private bool _isTidy = false;
    private bool _isMoving = false;
    private Outline _outline;

    private Vector3 _targetPos;
    private Quaternion _targetRot;

    void Awake()
    {
        _outline = GetComponent<Outline>();
        if (_outline != null) _outline.enabled = false;

        // PENGATURAN POSISI RAPI
        if (neatTarget == null)
        {
            // Jika target kosong, asumsikan "Rapi" = berdiri tegak di posisi saat ini.
            // Rotasi X dan Z di-nol-kan, sedangkan hadapan (Y) dibiarkan seperti semula.
            _targetPos = transform.position; 
            _targetRot = Quaternion.Euler(0f, transform.eulerAngles.y, 0f); 
        }
        else
        {
            // Jika ada target, ikuti posisi dan rotasi target tersebut
            _targetPos = neatTarget.position;
            _targetRot = neatTarget.rotation;
        }
    }

    // --- IInteractable ---
    public void OnLookAt()
    {
        if (!_isTidy && !_isMoving && _outline != null) 
            _outline.enabled = true;
    }

    public void OnLookAway()
    {
        if (_outline != null) 
            _outline.enabled = false;
    }

    public string GetPromptText()
    {
        if (_isTidy) return "";

        PlayerInteraction pi = FindAnyObjectByType<PlayerInteraction>();
        if (pi != null && pi.currentTool != ToolType.Tangan)
            return "Butuh Tangan Kosong (Tekan 1)";

        return "[E] Rapikan";
    }

    public void Interact(GameObject player)
    {
        if (_isTidy || _isMoving) return;

        PlayerInteraction pi = player.GetComponent<PlayerInteraction>();
        
        // Hanya bisa merapikan pakai Tangan Kosong
        if (pi != null && pi.currentTool == ToolType.Tangan)
        {
            StartCoroutine(TidyRoutine());
        }
    }

    // --- Animasi Merapikan ---
    IEnumerator TidyRoutine()
    {
        _isMoving = true;
        OnLookAway(); // Matikan highlight kuning

        if (slideSound != null)
            AudioSource.PlayClipAtPoint(slideSound, transform.position);

        Vector3 startPos = transform.position;
        Quaternion startRot = transform.rotation;
        float t = 0f;

        // Proses animasi bergeser mulus
        while (t < 1f)
        {
            t += Time.deltaTime / tidyDuration;
            transform.position = Vector3.Lerp(startPos, _targetPos, t);
            transform.rotation = Quaternion.Lerp(startRot, _targetRot, t);
            yield return null;
        }

        // Pastikan posisinya pas 100% di akhir animasi
        transform.position = _targetPos;
        transform.rotation = _targetRot;

        _isTidy = true;
        _isMoving = false;
        Debug.Log($"{name} berhasil dirapikan!");
    }
}