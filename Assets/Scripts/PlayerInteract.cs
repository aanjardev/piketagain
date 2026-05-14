using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    public float interactDistance = 8f;

    public GameObject interactText;

    void Update()
    {
        Ray ray = Camera.main.ViewportPointToRay(
            new Vector3(0.5f, 0.5f, 0)
        );

        // Debug.DrawRay(
        //     ray.origin,
        //     ray.direction * interactDistance,
        //     Color.red
        // );

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