using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class BgmVolumeController : MonoBehaviour
{
    private AudioSource _audioSource;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        // Jika SoundManager belum ada (mis. scene dimulai dari scene non-home), buat instance otomatis
        if (SoundManager.Instance == null)
        {
            var go = new GameObject("SoundManager");
            go.AddComponent<SoundManager>();
            // SoundManager akan memanggil DontDestroyOnLoad di Awake
        }

        SoundManager.Instance.SetBgmAudioSource(_audioSource);
        SoundManager.Instance.OnBGMVolumeChanged += UpdateVolume;
        UpdateVolume(SoundManager.Instance.BGMVolume);
    }

    private void OnDisable()
    {
        if (SoundManager.Instance == null) return;
        SoundManager.Instance.OnBGMVolumeChanged -= UpdateVolume;
    }

    private void UpdateVolume(float value)
    {
        if (_audioSource != null)
            _audioSource.volume = value;
    }
}
