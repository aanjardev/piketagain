using UnityEngine;
using UnityEngine.UI;

public class SettingsUI : MonoBehaviour
{
    [Header("--- Sliders ---")]
    public Slider sfxSlider;
    public Slider bgmSlider;
    public Slider sensitivitySlider;

    private void OnEnable()
    {
        if (SoundManager.Instance == null)
        {
            Debug.LogWarning("SoundManager tidak ditemukan. Pastikan ada SoundManager di scene awal.");
            return;
        }

        if (sfxSlider != null)
        {
            sfxSlider.SetValueWithoutNotify(SoundManager.Instance.SFXVolume);
            sfxSlider.onValueChanged.AddListener(SoundManager.Instance.SetSFXVolume);
        }

        if (bgmSlider != null)
        {
            bgmSlider.SetValueWithoutNotify(SoundManager.Instance.BGMVolume);
            bgmSlider.onValueChanged.AddListener(SoundManager.Instance.SetBGMVolume);
        }

        if (sensitivitySlider != null)
        {
            sensitivitySlider.SetValueWithoutNotify(SoundManager.Instance.Sensitivity);
            sensitivitySlider.onValueChanged.AddListener(SoundManager.Instance.SetSensitivity);
        }
    }

    private void OnDisable()
    {
        if (SoundManager.Instance == null) return;

        if (sfxSlider != null)
            sfxSlider.onValueChanged.RemoveListener(SoundManager.Instance.SetSFXVolume);

        if (bgmSlider != null)
            bgmSlider.onValueChanged.RemoveListener(SoundManager.Instance.SetBGMVolume);

        if (sensitivitySlider != null)
            sensitivitySlider.onValueChanged.RemoveListener(SoundManager.Instance.SetSensitivity);
    }
}
