using UnityEngine;
using TMPro;

public class PlayerInteract : MonoBehaviour
{
    public float interactDistance = 8f;

    public GameObject interactText;
    public string promptMessage = "Tekan E";

    private TextMeshProUGUI _promptText;

    void Awake()
    {
        if (interactText != null)
        {
            _promptText = interactText.GetComponentInChildren<TextMeshProUGUI>();
            if (_promptText != null)
                _promptText.text = promptMessage;

            interactText.SetActive(false);
        }
    }

    void Update()
    {
        if (interactText == null) return;

        Ray ray = Camera.main.ViewportPointToRay(
            new Vector3(0.5f, 0.5f, 0)
        );
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactDistance))
        {
            if (hit.collider.CompareTag("Interactable"))
            {
                interactText.SetActive(true);

                if (Input.GetKeyDown(KeyCode.E))
                {
                    Debug.Log("Interact");
                }
            }
            else
            {
                interactText.SetActive(false);
            }
        }
        else
        {
            interactText.SetActive(false);
        }
    }
}
