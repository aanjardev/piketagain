using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Attach to the Player GameObject.
/// Handles raycasting for interactable items (Books, Trash, Dust),
/// shows a UI prompt, and delegates the action to the item's own script.
/// </summary>
[RequireComponent(typeof(CharacterController))]
public class PlayerInteraction : MonoBehaviour
{
    [Header("--- Interaction Settings ---")]
    [Tooltip("How far the player can reach to interact.")]
    public float interactRange = 2.5f;
    [Tooltip("Layers the interaction raycast can hit (Items + Furniture).")]
    public LayerMask interactableLayers;
    [Tooltip("Key the player presses to interact.")]
    public KeyCode interactKey = KeyCode.E;

    [Header("--- Held Item ---")]
    [Tooltip("Empty child transform in front of the camera where held items sit.")]
    public Transform holdPoint;

    [Header("--- Throwing (Trash) ---")]
    [Tooltip("Force applied when throwing a trash item.")]
    public float throwForce = 6f;

    [Header("--- UI ---")]
    [Tooltip("The CanvasGroup wrapping the interaction prompt panel.")]
    public CanvasGroup promptPanel;
    [Tooltip("Text element showing the action hint, e.g. '[E] Pick up Book'.")]
    public Text promptText;

    // Internal state
    private IInteractable _targetInteractable; // item under the crosshair
    private GameObject    _heldItem;           // currently held object
    private IInteractable _heldInteractable;   // interface on held item

    private Camera _cam;

    // -----------------------------------------------------------------------
    void Awake()
    {
        _cam = Camera.main;
        HidePrompt();
    }

    // -----------------------------------------------------------------------
    void Update()
    {
        ScanForInteractable();

        if (Input.GetKeyDown(interactKey))
            TryInteract();
    }

    // -----------------------------------------------------------------------
    /// <summary>Fires a ray from the camera centre; highlights whatever it hits.</summary>
    void ScanForInteractable()
    {
        Ray ray = new Ray(_cam.transform.position, _cam.transform.forward);
        bool hit = Physics.Raycast(ray, out RaycastHit info, interactRange, interactableLayers);

        if (hit && info.collider.TryGetComponent(out IInteractable interactable))
        {
            // Same target — nothing to update
            if (interactable == _targetInteractable) return;

            _targetInteractable?.OnLookAway();
            _targetInteractable = interactable;
            _targetInteractable.OnLookAt();
            ShowPrompt(interactable.GetPromptText());
        }
        else
        {
            if (_targetInteractable != null)
            {
                _targetInteractable.OnLookAway();
                _targetInteractable = null;
            }

            // If we're holding something and not looking at a receptor, remind the player
            if (_heldItem != null)
                ShowPrompt($"[{interactKey}] Throw  |  Hold to aim");
            else
                HidePrompt();
        }
    }

    // -----------------------------------------------------------------------
    void TryInteract()
    {
        if (_heldItem != null && _heldItem.CompareTag("Trash"))
        {
            // Cek apakah pemain sedang melihat ke arah tong sampah
            if (_targetInteractable is TrashCan trashCan)
            {
                trashCan.ReceiveTrash(_heldItem.GetComponent<TrashItem>());
                
                // Kosongkan tangan pemain
                _heldItem = null;
                _heldInteractable = null;
            }
            else 
            {
                // Jika klik [E] tapi TIDAK melihat tong sampah, jatuhkan sampahnya ke lantai
                ThrowHeldItem(); 
            }
            return;
        }
        
        // --- If holding a Book and looking at a Bookshelf receptor ---
        if (_heldItem != null && _heldItem.CompareTag("Book") && _targetInteractable is BookshelfSlot)
        {
            _targetInteractable.Interact(gameObject);
            PlaceHeldItemAt(_targetInteractable as MonoBehaviour);
            return;
        }

        // --- No target — nothing to do ---
        if (_targetInteractable == null) return;

        // --- Delegate to the item ---
        _targetInteractable.Interact(gameObject);

        // Pick up if the item requests it
        if (_targetInteractable is IPickupable pickupable && pickupable.WantsPickup())
            PickUp((pickupable as MonoBehaviour).gameObject);
    }

    // -----------------------------------------------------------------------
    void PickUp(GameObject item)
    {
        if (_heldItem != null) return; // already holding something

        _heldItem = item;
        _heldInteractable = item.GetComponent<IInteractable>();

        // Disable physics while held
        if (item.TryGetComponent(out Rigidbody rb))
        {
            rb.isKinematic = true;
            rb.useGravity  = false;
        }

        // Disable collider so it doesn't bump the player
        if (item.TryGetComponent(out Collider col))
            col.enabled = false;

        item.transform.SetParent(holdPoint);
        item.transform.localPosition = Vector3.zero;
        item.transform.localRotation = Quaternion.identity;
    }

    // -----------------------------------------------------------------------
    void ThrowHeldItem()
    {
        if (_heldItem == null) return;

        _heldItem.transform.SetParent(null);

        if (_heldItem.TryGetComponent(out Collider col))
            col.enabled = true;

        if (_heldItem.TryGetComponent(out Rigidbody rb))
        {
            rb.isKinematic = false;
            rb.useGravity  = true;
            rb.AddForce(_cam.transform.forward * throwForce, ForceMode.Impulse);
        }

        _heldItem = null;
        _heldInteractable = null;
    }

    // -----------------------------------------------------------------------
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

    // -----------------------------------------------------------------------
    void ShowPrompt(string message)
    {
        if (promptPanel == null || promptText == null) return;
        promptText.text    = message;
        promptPanel.alpha  = 1f;
        promptPanel.blocksRaycasts = false;
    }

    void HidePrompt()
    {
        if (promptPanel == null) return;
        promptPanel.alpha = 0f;
    }
}