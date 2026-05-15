using UnityEngine;
using TMPro;

// Daftar alat yang bisa dipakai pemain
public enum ToolType { Tangan, KantongSampah, KainLap }

[RequireComponent(typeof(CharacterController))]
public class PlayerInteraction : MonoBehaviour
{
    [Header("--- Interaction Settings ---")]
    public float interactRange = 2.5f;
    public LayerMask interactableLayers;
    public KeyCode interactKey = KeyCode.E;

    [Header("--- Tools & Inventory ---")]
    public ToolType currentTool = ToolType.Tangan;
    public int kapasitasKantong = 5;
    public int isiKantongSaatIni = 0;

    [Header("--- Tool Visuals (Models) ---")]
    [Tooltip("Model 3D Kantong Sampah (jadikan anak dari HoldPoint)")]
    public GameObject kantongSampahModel;

    [Tooltip("Model 3D Kain Lap (jadikan anak dari HoldPoint)")]
    public GameObject kainLapModel;

    [Header("--- Held Item (Khusus Buku) ---")]
    public Transform holdPoint;
    public float dropForce = 2f; // Mengganti throwForce karena buku tidak dilempar kencang

    [Header("--- UI ---")]
    [Tooltip("Panel prompt interaksi. Jangan masukkan crosshair ke dalam panel ini.")]
    public CanvasGroup promptPanel;

    [Tooltip("Text untuk prompt interaksi, contoh: '[E] Masukkan ke Kantong'.")]
    public TextMeshProUGUI promptText;

    [Tooltip("Crosshair/pointer yang harus selalu terlihat.")]
    public GameObject crosshair;

    [Tooltip("CanvasGroup untuk Progress List UI yang muncul saat Tab ditahan.")]
    public CanvasGroup progressListPanel;

    [Header("--- Emptying Trash (Hold Q) ---")]
    public float emptyHoldDuration = 2.0f; // Butuh hold 2 detik
    private float _emptyProgress = 0f;
    private bool _isEmptying = false;

    // Internal state
    private IInteractable _targetInteractable;
    private GameObject _heldItem;
    private IInteractable _heldInteractable;
    private Camera _cam;

    public GameObject heldItem => _heldItem;

    // -----------------------------------------------------------------------
    void Awake()
    {
        _cam = Camera.main;

        // Crosshair harus selalu aktif
        if (crosshair != null)
            crosshair.SetActive(true);

        HidePrompt();
        HideProgressList();
        UpdateToolVisuals();
    }

    // -----------------------------------------------------------------------
    void Update()
    {
        HandleToolSwitching();
        ScanForInteractable();

        if (Input.GetKeyDown(interactKey))
            TryInteract();

        // Mengecek apakah pemain sedang menahan tombol Q
        HandleEmptyTrashLogic();

        // Handle Tab key to show/hide ProgressList
        if (Input.GetKeyDown(KeyCode.Tab))
            ShowProgressList();

        if (Input.GetKeyUp(KeyCode.Tab))
            HideProgressList();

        // Jaga-jaga supaya crosshair tidak mati
        if (crosshair != null && !crosshair.activeSelf)
            crosshair.SetActive(true);
    }

    // -----------------------------------------------------------------------
    #region TOOLS SYSTEM

    void HandleToolSwitching()
    {
        // Cegah ganti alat kalau pemain masih memegang buku fisik
        if (_heldItem != null) return;

        if (Input.GetKeyDown(KeyCode.Alpha1)) ChangeTool(ToolType.Tangan);
        if (Input.GetKeyDown(KeyCode.Alpha2)) ChangeTool(ToolType.KantongSampah);
        if (Input.GetKeyDown(KeyCode.Alpha3)) ChangeTool(ToolType.KainLap);
    }

    void ChangeTool(ToolType newTool)
    {
        if (currentTool == newTool) return;

        currentTool = newTool;
        UpdateToolVisuals();
        Debug.Log("Ganti alat ke: " + currentTool);
    }

    void UpdateToolVisuals()
    {
        if (kantongSampahModel != null)
            kantongSampahModel.SetActive(currentTool == ToolType.KantongSampah);

        if (kainLapModel != null)
            kainLapModel.SetActive(currentTool == ToolType.KainLap);
    }

    #endregion

    // -----------------------------------------------------------------------
    #region INTERACTION LOGIC

    void ScanForInteractable()
    {
        if (_cam == null)
        {
            _cam = Camera.main;
            if (_cam == null) return;
        }

        Ray ray = new Ray(_cam.transform.position, _cam.transform.forward);
        bool hit = Physics.Raycast(ray, out RaycastHit info, interactRange, interactableLayers);

        if (hit && info.collider.TryGetComponent(out IInteractable interactable))
        {
            if (interactable != _targetInteractable)
            {
                _targetInteractable?.OnLookAway();
                _targetInteractable = interactable;
                _targetInteractable.OnLookAt();
            }

            string prompt = interactable.GetPromptText();

            // UI tambahan kalau sedang pakai Kantong Sampah
            if (currentTool == ToolType.KantongSampah)
            {
                if (interactable is TrashItem)
                {
                    prompt += $" ({isiKantongSaatIni}/{kapasitasKantong})";
                }
                else if (interactable is TrashCan tc && tc.IsOpen && isiKantongSaatIni > 0)
                {
                    if (_isEmptying)
                        prompt += $"\n[Q] Membuang... {Mathf.RoundToInt(_emptyProgress * 300)}%";
                    else
                        prompt += "\nTahan [Q] Buang Sampah";
                }
            }

            ShowPrompt(prompt);
        }
        else
        {
            if (_targetInteractable != null)
            {
                _targetInteractable.OnLookAway();
                _targetInteractable = null;
            }

            _isEmptying = false;
            _emptyProgress = 0f;

            if (_heldItem != null)
                ShowPrompt($"[{interactKey}] Jatuhkan Buku");
            else
                HidePrompt();
        }
    }

    void TryInteract()
    {
        // 1. Kalau sedang memegang buku fisik
        if (_heldItem != null)
        {
            if (_targetInteractable is BookshelfSlot)
            {
                _targetInteractable.Interact(gameObject);
                PlaceHeldItemAt(_targetInteractable as MonoBehaviour);
            }
            else
            {
                ThrowHeldItem();
            }

            return;
        }

        // 2. Kalau tidak ada target, tidak melakukan apa-apa
        if (_targetInteractable == null) return;

        // 3. Jalankan interaksi object yang sedang dilihat
        _targetInteractable.Interact(gameObject);

        // 4. Khusus buku: ambil buku jika sedang pakai tangan kosong
        if (currentTool == ToolType.Tangan &&
            _targetInteractable is IPickupable pickupable &&
            pickupable.WantsPickup())
        {
            PickUp((pickupable as MonoBehaviour).gameObject);
        }
    }

    #endregion

    // -----------------------------------------------------------------------
    #region ITEM HANDLING (Buku)

    void PickUp(GameObject item)
    {
        if (_heldItem != null) return;

        _heldItem = item;
        _heldInteractable = item.GetComponent<IInteractable>();

        if (item.TryGetComponent(out Rigidbody rb))
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        if (item.TryGetComponent(out Collider col))
            col.enabled = false;

        item.transform.SetParent(holdPoint);
        item.transform.localPosition = Vector3.zero;
        item.transform.localRotation = Quaternion.identity;
    }

    void ThrowHeldItem()
    {
        if (_heldItem == null) return;

        _heldItem.transform.SetParent(null);

        if (_heldItem.TryGetComponent(out Collider col))
            col.enabled = true;

        if (_heldItem.TryGetComponent(out Rigidbody rb))
        {
            rb.isKinematic = false;
            rb.useGravity = true;
            rb.AddForce(_cam.transform.forward * dropForce, ForceMode.Impulse);
        }

        _heldItem = null;
        _heldInteractable = null;
    }

    void PlaceHeldItemAt(MonoBehaviour receptor)
    {
        if (_heldItem == null || receptor == null) return;

        _heldItem.transform.SetParent(receptor.transform);
        _heldItem.transform.localPosition = Vector3.zero;
        _heldItem.transform.localRotation = Quaternion.identity;

        if (_heldItem.TryGetComponent(out Collider col))
            col.enabled = true;

        _heldItem = null;
        _heldInteractable = null;
    }

    void HandleEmptyTrashLogic()
    {
        // Syarat: pakai kantong, isi > 0, lihat TrashCan, dan TrashCan terbuka
        if (_targetInteractable is TrashCan tc &&
            tc.IsOpen &&
            currentTool == ToolType.KantongSampah &&
            isiKantongSaatIni > 0)
        {
            if (Input.GetKey(KeyCode.Q))
            {
                _isEmptying = true;
                _emptyProgress += Time.deltaTime / emptyHoldDuration;

                if (_emptyProgress >= 1f)
                {
                    tc.EmptyBagIntoCan(isiKantongSaatIni);
                    isiKantongSaatIni = 0;
                    _emptyProgress = 0f;
                    _isEmptying = false;

                    // Setelah buang sampah, otomatis balik ke tangan kosong
                    ChangeTool(ToolType.Tangan);
                }
            }
            else
            {
                _isEmptying = false;
                _emptyProgress = 0f;
            }
        }
        else
        {
            _isEmptying = false;
            _emptyProgress = 0f;
        }
    }

    #endregion

    // -----------------------------------------------------------------------
    #region UI

    void ShowPrompt(string message)
    {
        if (promptPanel == null || promptText == null) return;

        promptText.text = message;
        promptPanel.alpha = 1f;
        promptPanel.blocksRaycasts = false;
        promptPanel.interactable = false;
    }

    void HidePrompt()
    {
        if (promptPanel == null) return;

        promptPanel.alpha = 0f;
        promptPanel.blocksRaycasts = false;
        promptPanel.interactable = false;
    }

    void ShowProgressList()
    {
        if (progressListPanel == null) return;

        progressListPanel.alpha = 1f;
        progressListPanel.blocksRaycasts = true;
        progressListPanel.interactable = true;
    }

    void HideProgressList()
    {
        if (progressListPanel == null) return;

        progressListPanel.alpha = 0f;
        progressListPanel.blocksRaycasts = false;
        progressListPanel.interactable = false;
    }

    #endregion
}