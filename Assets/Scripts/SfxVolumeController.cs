using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SfxVolumeController : MonoBehaviour
{
    [Header("--- Base Volume ---")]
    [Tooltip("Volume dasar audio source ini sebelum dikalikan dengan global SFX volume.")]
    public float baseVolume = 1f;

    private AudioSource _audioSource;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        if (SoundManager.Instance == null)
        {
            var go = new GameObject("SoundManager");
            go.AddComponent<SoundManager>();
        }

        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.OnSFXVolumeChanged += UpdateVolume;
            UpdateVolume(SoundManager.Instance.SFXVolume);
        }
    }

    private void OnDisable()
    {
        if (SoundManager.Instance == null) return;
        SoundManager.Instance.OnSFXVolumeChanged -= UpdateVolume;
    }

    private void UpdateVolume(float sfxVolume)
    {
        if (_audioSource == null) return;
        _audioSource.volume = baseVolume * sfxVolume;
    }
}
