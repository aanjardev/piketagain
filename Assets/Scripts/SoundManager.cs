using System;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [Header("--- Audio Sources ---")]
    [Tooltip("Referensi ke AudioSource BGM jika tersedia. Jika tidak, gunakan BgmVolumeController pada AudioSource masing-masing scene.")]
    public AudioSource bgmAudioSource;

    private const string SfxKey = "SFXVolume";
    private const string BgmKey = "BGMVolume";
    private const string SensKey = "Sensitivity";

    public static SoundManager Instance { get; private set; }

    public event Action<float> OnSFXVolumeChanged;
    public event Action<float> OnBGMVolumeChanged;
    public event Action<float> OnSensitivityChanged;

    public float SFXVolume { get; private set; } = 1f;
    public float BGMVolume { get; private set; } = 1f;
    public float Sensitivity { get; private set; } = 1f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        LoadSettings();
    }

    private void Start()
    {
        ApplyBgmVolume();
    }

    private void LoadSettings()
    {
        SFXVolume = PlayerPrefs.GetFloat(SfxKey, 1f);
        BGMVolume = PlayerPrefs.GetFloat(BgmKey, 1f);
        Sensitivity = PlayerPrefs.GetFloat(SensKey, 1f);
    }

    private void SaveSettings()
    {
        PlayerPrefs.SetFloat(SfxKey, SFXVolume);
        PlayerPrefs.SetFloat(BgmKey, BGMVolume);
        PlayerPrefs.SetFloat(SensKey, Sensitivity);
        PlayerPrefs.Save();
    }

    public void SetSFXVolume(float value)
    {
        SFXVolume = Mathf.Clamp01(value);
        SaveSettings();
        OnSFXVolumeChanged?.Invoke(SFXVolume);
    }

    public void SetBGMVolume(float value)
    {
        BGMVolume = Mathf.Clamp01(value);
        SaveSettings();
        ApplyBgmVolume();
        OnBGMVolumeChanged?.Invoke(BGMVolume);
    }

    public void SetSensitivity(float value)
    {
        // Terima nilai sensitivity yang lebih besar dari 1 (mis. hingga 5)
        Sensitivity = Mathf.Clamp(value, 0.1f, 5f);
        SaveSettings();
        OnSensitivityChanged?.Invoke(Sensitivity);
    }

    public void ApplyBgmVolume()
    {
        if (bgmAudioSource != null)
            bgmAudioSource.volume = BGMVolume;
    }

    public void SetBgmAudioSource(AudioSource source)
    {
        if (source == null) return;
        bgmAudioSource = source;
        ApplyBgmVolume();
    }

    public float GetSFXVolume() => SFXVolume;
    public float GetBGMVolume() => BGMVolume;
    public float GetSensitivity() => Sensitivity;

    public void PlaySFX(AudioSource audioSource, AudioClip clip, float volume = 1f)
    {
        if (audioSource == null || clip == null) return;

        audioSource.PlayOneShot(clip, volume * SFXVolume);
    }

    public void PlaySFXAtPoint(AudioClip clip, Vector3 position, float volume = 1f)
    {
        if (clip == null) return;

        GameObject temp = new GameObject("SFXTempAudio");
        temp.transform.position = position;

        AudioSource source = temp.AddComponent<AudioSource>();
        source.clip = clip;
        source.spatialBlend = 1f;
        source.volume = Mathf.Clamp01(volume * SFXVolume);
        source.Play();

        Destroy(temp, clip.length + 0.1f);
    }
}

