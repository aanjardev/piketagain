using UnityEngine;

/// <summary>
/// Centralized Audio Library Manager
/// Semua SFX disimpan di satu tempat.
/// Script lain tinggal reference manager ini untuk memutar SFX.
/// 
/// SETUP:
/// 1. Buat empty GameObject bernama "AudioLibrary"
/// 2. Attach script ini ke GameObject tersebut
/// 3. Assign semua file audio ke field yang sesuai di Inspector
/// 4. Selesai! Semua script akan otomatis menggunakan SFX dari sini.
/// </summary>
public class AudioLibrary : MonoBehaviour
{
    public static AudioLibrary Instance { get; private set; }

    [Header("=== SCHOOL & TIMER ===")]
    [SerializeField] public AudioClip bellSchoolSound;

    [Header("=== BOOKS ===")]
    [SerializeField] public AudioClip bookPickupSound;
    [SerializeField] public AudioClip bookShelveSound;

    [Header("=== TRASH ===")]
    [SerializeField] public AudioClip trashPickupSound;
    [SerializeField] public AudioClip trashDisposeSound;

    [Header("=== CLEANING ===")]
    [SerializeField] public AudioClip dustWipeSound;

    [Header("=== FURNITURE ===")]
    [SerializeField] public AudioClip furnitureSlideSound;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        Debug.Log("AudioLibrary Manager initialized!");
    }

    // ===== HELPER METHODS =====
    // Method untuk memutar SFX dengan mudah

    public void PlayBellSchool(Vector3 position)
    {
        PlaySFX(bellSchoolSound, position);
    }

    public void PlayBookPickup(Vector3 position)
    {
        PlaySFX(bookPickupSound, position);
    }

    public void PlayBookShelve(Vector3 position)
    {
        PlaySFX(bookShelveSound, position);
    }

    public void PlayTrashPickup(Vector3 position)
    {
        PlaySFX(trashPickupSound, position);
    }

    public void PlayTrashDispose(Vector3 position)
    {
        PlaySFX(trashDisposeSound, position);
    }

    public void PlayDustWipe(Vector3 position)
    {
        PlaySFX(dustWipeSound, position);
    }

    public void PlayFurnitureSlide(Vector3 position)
    {
        PlaySFX(furnitureSlideSound, position);
    }

    // Generic method untuk play SFX
    private void PlaySFX(AudioClip clip, Vector3 position, float volume = 1f)
    {
        if (clip == null)
        {
            Debug.LogWarning("AudioClip is null!");
            return;
        }

        SoundManager.Instance?.PlaySFXAtPoint(clip, position, volume);
    }
}
