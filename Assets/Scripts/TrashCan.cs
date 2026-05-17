using System.Collections;
using UnityEngine;

public class TrashCan : MonoBehaviour, IInteractable
{
    [Header("--- Visual & Animation ---")]
    public Animator animator;
    public ParticleSystem binParticles;
    public AudioClip emptyBagSound;

    [Header("--- Animation Timing ---")]
    [Tooltip("Durasi animasi buka. Jika Buka frame 1-30 dan 24 FPS, isi sekitar 1.2")]
    public float openDuration = 1.2f;

    [Tooltip("Durasi animasi tutup. Jika Tutup frame 80-100 dan 24 FPS, isi sekitar 0.8")]
    public float closeDuration = 0.8f;

    [Header("--- Optional Auto Close ---")]
    public bool autoCloseAfterEmpty = false;
    public float autoCloseDelay = 0.5f;

    private int _totalSampahDiTong;
    private Outline _outline;

    private bool _isAnimating;
    private bool _isOpening;
    private Coroutine _animationRoutine;
    private Coroutine _autoCloseRoutine;

    private static readonly int IsOpenHash = Animator.StringToHash("IsOpen");

    // Status ini dibaca oleh PlayerInteraction.
    // True hanya saat tong sudah benar-benar terbuka.
    public bool IsOpen { get; private set; }

    void Awake()
    {
        _outline = GetComponent<Outline>();

        if (_outline != null)
            _outline.enabled = false;

        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        IsOpen = false;
        _isAnimating = false;
        _isOpening = false;

        if (animator != null)
            animator.SetBool(IsOpenHash, false);
    }

    public void OnLookAt()
    {
        if (_outline != null)
            _outline.enabled = true;
    }

    public void OnLookAway()
    {
        if (_outline != null)
            _outline.enabled = false;
    }

    public string GetPromptText()
    {
        if (_isAnimating)
        {
            return _isOpening ? "Membuka Tong..." : "Menutup Tong...";
        }

        return IsOpen ? "[E] Tutup Tong" : "[E] Buka Tong";
    }

    public void Interact(GameObject player)
    {
        // Cegah tombol E ditekan berulang saat animasi masih berjalan.
        if (_isAnimating)
            return;

        if (IsOpen)
            CloseCan();
        else
            OpenCan();
    }

    public void OpenCan()
    {
        if (_isAnimating || IsOpen)
            return;

        if (_autoCloseRoutine != null)
        {
            StopCoroutine(_autoCloseRoutine);
            _autoCloseRoutine = null;
        }

        if (_animationRoutine != null)
            StopCoroutine(_animationRoutine);

        _animationRoutine = StartCoroutine(OpenRoutine());
    }

    public void CloseCan()
    {
        if (_isAnimating || !IsOpen)
            return;

        if (_animationRoutine != null)
            StopCoroutine(_animationRoutine);

        _animationRoutine = StartCoroutine(CloseRoutine());
    }

    private IEnumerator OpenRoutine()
    {
        _isAnimating = true;
        _isOpening = true;

        if (animator != null)
            animator.SetBool(IsOpenHash, true);

        yield return new WaitForSeconds(openDuration);

        IsOpen = true;
        _isAnimating = false;
        _isOpening = false;
        _animationRoutine = null;
    }

    private IEnumerator CloseRoutine()
    {
        _isAnimating = true;
        _isOpening = false;

        // Langsung false supaya PlayerInteraction tidak bisa buang sampah saat tong mulai menutup.
        IsOpen = false;

        if (animator != null)
            animator.SetBool(IsOpenHash, false);

        yield return new WaitForSeconds(closeDuration);

        _isAnimating = false;
        _animationRoutine = null;
    }

    // Fungsi ini dipanggil oleh PlayerInteraction saat selesai menahan [Q]
    public void EmptyBagIntoCan(int amount)
    {
        if (!IsOpen || _isAnimating)
        {
            Debug.LogWarning("Tong belum terbuka penuh, sampah belum bisa dibuang.");
            return;
        }

        if (amount <= 0)
            return;

        _totalSampahDiTong += amount;

        if (binParticles != null)
            binParticles.Play();

        if (emptyBagSound != null)
            SoundManager.Instance?.PlaySFXAtPoint(emptyBagSound, transform.position, 1f);

        Debug.Log($"Berhasil membuang {amount} sampah! Total di tong: {_totalSampahDiTong}");

        if (autoCloseAfterEmpty)
        {
            if (_autoCloseRoutine != null)
                StopCoroutine(_autoCloseRoutine);

            _autoCloseRoutine = StartCoroutine(AutoCloseRoutine());
        }
    }

    private IEnumerator AutoCloseRoutine()
    {
        yield return new WaitForSeconds(autoCloseDelay);

        if (IsOpen && !_isAnimating)
            CloseCan();

        _autoCloseRoutine = null;
    }
}