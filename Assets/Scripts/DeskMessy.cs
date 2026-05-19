using UnityEngine;
using System.Collections;

public class DeskMessy : MonoBehaviour, IInteractable
{
    [Header("Pengaturan Berantakan")]
    public float maxPositionOffset = 0.5f; 
    public float maxRotationAngle = 20f; 
    public LayerMask obstacleLayer; 

    [Header("Pengaturan Rapikan")]
    public float tidyDuration = 0.4f;

    private Vector3 _originalPosition;
    private Quaternion _originalRotation;
    private Collider _myCollider;
    
    private bool _isTidy = true;
    private bool _isMoving = false;
    private Outline _outline;

    public bool IsMessy => !_isTidy;

    void Awake()
    {
        _originalPosition = transform.position;
        _originalRotation = transform.rotation;
        _myCollider = GetComponent<Collider>(); 

        _outline = GetComponent<Outline>();
        if (_outline != null) _outline.enabled = false;
    }

    // Fungsi ini dipanggil oleh Manager
    public void MakeItMessy()
    {
        _isTidy = false;
        int maxAttempts = 10; 
        bool positionFound = false;

        for (int i = 0; i < maxAttempts; i++)
        {
            float randomX = Random.Range(-maxPositionOffset, maxPositionOffset);
            float randomZ = Random.Range(-maxPositionOffset, maxPositionOffset);
            Vector3 testPosition = _originalPosition + new Vector3(randomX, 0, randomZ);

            float randomRotY = Random.Range(-maxRotationAngle, maxRotationAngle);
            Quaternion testRotation = _originalRotation * Quaternion.Euler(0, randomRotY, 0);

            if (_myCollider != null)
            {
                _myCollider.enabled = false;
                Vector3 centerOffset = _myCollider.bounds.center - transform.position;
                Vector3 checkCenter = testPosition + centerOffset;

                bool isColliding = Physics.CheckBox(
                    checkCenter,
                    _myCollider.bounds.extents, 
                    testRotation,
                    obstacleLayer
                );

                _myCollider.enabled = true;

                if (!isColliding) 
                {
                    transform.position = testPosition;
                    transform.rotation = testRotation;
                    positionFound = true;
                    break; 
                }
            }
        }

        if (!positionFound)
        {
            float randomRotY = Random.Range(-maxRotationAngle, maxRotationAngle);
            transform.rotation = _originalRotation * Quaternion.Euler(0, randomRotY, 0);
        }
    }

    // --- IInteractable ---
    public void OnLookAt() { if (!_isTidy && !_isMoving && _outline != null) _outline.enabled = true; }
    public void OnLookAway() { if (_outline != null) _outline.enabled = false; }
    public string GetPromptText()
    {
        if (_isTidy) return "";
        PlayerInteraction pi = FindAnyObjectByType<PlayerInteraction>();
        if (pi != null && pi.currentTool != ToolType.Tangan) return "Butuh Tangan Kosong (Tekan 1)";
        return "[E] Rapikan Meja";
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

        AudioLibrary.Instance?.PlayFurnitureSlide(transform.position);

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
        // Laporkan ke manager progress
        CleaningProgressManager.Instance?.ReportDeskComplete();
    }
}