using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRaycast : MonoBehaviour
{
    [SerializeField] private float rayDistance = 10f;
    private string targetTag = "Interactable";
    private IInteractable currentInteractable;
    CanvasGroup canvasGroup;

    private void Update()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        currentInteractable = null;

        if (Physics.Raycast(ray, out hit, rayDistance))
        {
            if (hit.collider.CompareTag(targetTag))
            {
                canvasGroup = hit.collider.GetComponentInChildren<CanvasGroup>();
                canvasGroup.alpha = 1;
                currentInteractable = hit.collider.GetComponent<IInteractable>();
            }
            else
            {
                if (canvasGroup != null)
                {
                    canvasGroup.alpha = 0;
                }
            }
            Debug.Log(canvasGroup);
        }
        else
        {
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 0;
            }
        }

        if (currentInteractable != null && Input.GetKeyDown(KeyCode.E))
        {
            currentInteractable.Interact();
        }
    }
}
