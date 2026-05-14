using System.Collections;
using UnityEngine;

/// <summary>
/// Attach to every Dust prefab (flat quad / decal on a surface).
/// The player looks at it and holds [E] to wipe it clean.
/// </summary>
public class DustItem : MonoBehaviour, IInteractable
{
    [Header("--- Wipe Settings ---")]
    [Tooltip("How long the player must hold [E] to fully wipe the dust.")]
    public float wipeDuration = 1.5f;
    [Tooltip("Key that triggers wiping (must match PlayerInteraction.interactKey).")]
    public KeyCode wipeKey = KeyCode.E;

    [Header("--- Visual Feedback ---")]
    [Tooltip("Renderer of the dust patch — used to fade it out.")]
    public Renderer dustRenderer;

    [Header("--- Audio ---")]
    public AudioClip wipeSound;

    private bool  _isCleaned;
    private bool  _isWiping;
    private float _wipeProgress; // 0 → 1
    private AudioSource _audioSource;

    // -----------------------------------------------------------------------
    void Awake()
    {
        if (wipeSound != null)
        {
            _audioSource = gameObject.AddComponent<AudioSource>();
            _audioSource.clip   = wipeSound;
            _audioSource.loop   = true;
            _audioSource.volume = 0.6f;
        }
    }

    // -----------------------------------------------------------------------
    void Update()
    {
        if (_isCleaned) return;

        if (_isWiping)
        {
            if (Input.GetKey(wipeKey))
            {
                _wipeProgress += Time.deltaTime / wipeDuration;
                UpdateDustAlpha();

                if (_wipeProgress >= 1f)
                    FinishWipe();
            }
            else
            {
                // Player released key — slowly regress
                _wipeProgress -= Time.deltaTime / wipeDuration * 0.5f;
                _wipeProgress  = Mathf.Max(0f, _wipeProgress);
                UpdateDustAlpha();
                StopWipeAudio();
            }
        }
    }

    // -----------------------------------------------------------------------
    #region IInteractable

    public void OnLookAt()
    {
        if (_isCleaned) return;
        _isWiping = true;
        StartWipeAudio();
    }

    public void OnLookAway()
    {
        _isWiping = false;
        StopWipeAudio();
    }

    public string GetPromptText() =>
        _isCleaned ? "" : $"Hold [E] to wipe  ({Mathf.RoundToInt(_wipeProgress * 100)}%)";

    public void Interact(GameObject player)
    {
        // Wiping is time-based (held key), so this is a no-op.
        // The actual logic lives in Update() once OnLookAt() sets _isWiping = true.
    }

    #endregion

    // -----------------------------------------------------------------------
    void UpdateDustAlpha()
    {
        if (dustRenderer == null) return;

        // Fade the dust material's alpha based on wipe progress
        Color c = dustRenderer.material.color;
        c.a = Mathf.Lerp(1f, 0f, _wipeProgress);
        dustRenderer.material.color = c;
    }

    void FinishWipe()
    {
        _isCleaned = true;
        _isWiping  = false;
        StopWipeAudio();
        Debug.Log($"{name} wiped clean.");

        // Fade out completely, then destroy
        StartCoroutine(FadeAndDestroy());
    }

    IEnumerator FadeAndDestroy()
    {
        float t = 0f;
        while (t < 0.4f)
        {
            t += Time.deltaTime;
            UpdateDustAlpha();
            yield return null;
        }
        Destroy(gameObject);
    }

    void StartWipeAudio()
    {
        if (_audioSource != null && !_audioSource.isPlaying)
            _audioSource.Play();
    }

    void StopWipeAudio()
    {
        if (_audioSource != null && _audioSource.isPlaying)
            _audioSource.Stop();
    }
}