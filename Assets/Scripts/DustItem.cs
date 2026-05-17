using System.Collections;
using UnityEngine;

public class DustItem : MonoBehaviour, IInteractable
{
    [Header("--- Wipe Settings ---")]
    public float wipeDuration = 1.5f;
    public KeyCode wipeKey = KeyCode.E;

    // [Header("--- Visual Feedback ---")]
    // public Renderer dustRenderer;

    [Header("--- Audio ---")]
    public AudioClip wipeSound;

    private bool  _isCleaned;
    private bool  _isWiping;
    private float _wipeProgress; 
    private Material _dustMaterial;
    private float _lastAlpha;
    private AudioSource _audioSource;
    private PlayerInteraction _player; // Untuk mengecek alat pemain

    void Awake()
    {
        // Mencari script pemain secara otomatis
        _player = FindAnyObjectByType<PlayerInteraction>();

        if (wipeSound != null)
        {
            _audioSource = gameObject.AddComponent<AudioSource>();
            _audioSource.clip   = wipeSound;
            _audioSource.loop   = true;
            _audioSource.volume = 1f;

            var controller = _audioSource.gameObject.AddComponent<SfxVolumeController>();
            controller.baseVolume = 0.6f;
        }
    }

    void Update()
    {
        if (_isCleaned) return;

        bool canWipe = _isWiping && _player != null && _player.currentTool == ToolType.KainLap;

        if (canWipe && Input.GetKey(wipeKey))
        {
            _wipeProgress += Time.deltaTime / wipeDuration;
            _wipeProgress = Mathf.Clamp01(_wipeProgress);

            UpdateDustAlpha();
            StartWipeAudio();

            if (_wipeProgress >= 1f)
                FinishWipe();
        }
        else
        {
            if (_wipeProgress > 0f)
            {
                _wipeProgress -= Time.deltaTime / wipeDuration * 0.5f;
                _wipeProgress = Mathf.Max(0f, _wipeProgress);
                UpdateDustAlpha();
            }

            StopWipeAudio();
        }
    }

    // --- IInteractable ---
    public void OnLookAt() { _isWiping = true; }
    public void OnLookAway() { _isWiping = false; }

    public string GetPromptText()
    {
        if (_isCleaned) return "";

        // Ubah teks berdasarkan alat yang dipakai
        if (_player != null && _player.currentTool != ToolType.KainLap)
            return "Butuh Kain Lap (Tekan 3)";

        return $"Tahan [E] untuk Lap  ({Mathf.RoundToInt(_wipeProgress * 100)}%)";
    }

    public void Interact(GameObject player) { /* Dikosongkan karena pakai sistem Hold di Update */ }

    // --- Helpers ---
    void UpdateDustAlpha()
    {
        if (_dustMaterial == null) return;

        float alpha = Mathf.Lerp(1f, 0f, _wipeProgress);

        if (Mathf.Approximately(alpha, _lastAlpha)) return;

        _lastAlpha = alpha;

        Color c = _dustMaterial.GetColor("_BaseColor");
        c.a = alpha;
        _dustMaterial.SetColor("_BaseColor", c);
    }

    IEnumerator FadeAndDestroy()
    {
        float t = 0f;
        while (t < 0.4f)
        {
            t += Time.deltaTime;
            // Kita skip UpdateDustAlpha di sini agar tidak bentrok, 
            // biarkan debu langsung hancur setelah animasi lap selesai.
            yield return null;
        }
        Destroy(gameObject);
    }    
    
    void FinishWipe()
    {
        _isCleaned = true;
        _isWiping  = false;
        StopWipeAudio();
    //ada konflik disini
        Debug.Log($"{name} wiped clean.");

        CleaningProgressManager.Instance?.ReportDustComplete();
    //sampe sini

        StartCoroutine(FadeAndDestroy());
    }

    void StartWipeAudio()
    {
        if (_audioSource != null && !_audioSource.isPlaying) _audioSource.Play();
    }

    void StopWipeAudio()
    {
        if (_audioSource != null && _audioSource.isPlaying) _audioSource.Stop();
    }
}