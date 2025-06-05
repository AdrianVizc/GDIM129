using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRaycast : MonoBehaviour
{
    [SerializeField] private float rayDistance = 10f;
    [SerializeField] private GameObject revealObject;
    private string targetTag = "Interactable";
    private IInteractable currentInteractable;

    private void Update()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        currentInteractable = null;

        if (Physics.Raycast(ray, out hit, rayDistance))
        {
            if (hit.collider.CompareTag(targetTag))
            {
                revealObject.SetActive(true);
                currentInteractable = hit.collider.GetComponent<IInteractable>();
            }
            else
            {
                revealObject.SetActive(false);
            }
        }
        else
        {
            revealObject.SetActive(false);
        }

        if (currentInteractable != null && Input.GetKeyDown(KeyCode.E))
        {
            currentInteractable.Interact();
        }
    }
}
