using UnityEngine;
using System.Collections;

public class ChairMessy : MonoBehaviour, IInteractable
{
    [Header("Pengaturan Jatuh")]
    public float liftHeight = 0.5f; 
    public float tumbleForce = 15f; 
    public float settleTime = 2.5f;

    [Header("Pengaturan Rapikan")]
    public float tidyDuration = 0.4f;

    private Vector3 _originalPosition;
    private Quaternion _originalRotation;
    private Rigidbody _rb;
    
    private bool _isTidy = true;
    private bool _isMoving = false;
    private Outline _outline;

    void Awake()
    {
        _originalPosition = transform.position;
        _originalRotation = transform.rotation;
        _rb = GetComponent<Rigidbody>();

        _outline = GetComponent<Outline>();
        if (_outline != null) _outline.enabled = false;
    }

    // Fungsi ini dipanggil oleh Manager
    public void DropAndSettle()
    {
        _isTidy = false;
        StartCoroutine(DropRoutine());
    }

    IEnumerator DropRoutine()
    {
        _rb.isKinematic = false;
        _rb.useGravity = true;

        transform.position += new Vector3(0, liftHeight, 0);

        Vector3 randomTumble = new Vector3(
            Random.Range(-tumbleForce, tumbleForce),
            Random.Range(-tumbleForce, tumbleForce),
            Random.Range(-tumbleForce, tumbleForce)
        );
        _rb.AddTorque(randomTumble, ForceMode.Impulse);

        yield return new WaitForSeconds(settleTime);
        _rb.isKinematic = true;
    }

    // --- IInteractable ---
    public void OnLookAt() { if (!_isTidy && !_isMoving && _outline != null) _outline.enabled = true; }
    public void OnLookAway() { if (_outline != null) _outline.enabled = false; }
    public string GetPromptText()
    {
        if (_isTidy) return "";
        PlayerInteraction pi = FindObjectOfType<PlayerInteraction>();
        if (pi != null && pi.currentTool != ToolType.Tangan) return "Butuh Tangan Kosong (Tekan 1)";
        return "[E] Tegakkan Kursi";
    }

    public void Interact(GameObject player)
    {
        PlayerInteraction pi = player.GetComponent<PlayerInteraction>();
        if (!_isTidy && !_isMoving && pi != null && pi.currentTool == ToolType.Tangan)
            StartCoroutine(Luruskan());
    }

    IEnumerator Luruskan()
    {
        _isMoving = true;
        OnLookAway();
        
        _rb.isKinematic = true; // Pastikan fisika mati saat merapikan

        Vector3 startPos = transform.position;
        Quaternion startRot = transform.rotation;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime / tidyDuration;
            transform.position = Vector3.Lerp(startPos, _originalPosition, t);
            transform.rotation = Quaternion.Lerp(startRot, _originalRotation, t);
            yield return null;
        }

        transform.position = _originalPosition;
        transform.rotation = _originalRotation;
        _isTidy = true;
        _isMoving = false;
    }
}