using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] private float interactDistance;
    [SerializeField] KeyCode interactKey;
    [SerializeField] private Camera playerCam;

    private IInteractable interactingObj;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(interactKey))
        {
            //check for interactable object
            CheckIfInteractable();

            if (interactingObj != null)
            {
                interactingObj.Interact();
            }
        }
    }

    private void CheckIfInteractable()
    {
        Ray ray = playerCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactDistance))
        {
            interactingObj = hit.collider.GetComponent<IInteractable>();
            Debug.Log("Hit: " + interactingObj);
        }
        else
        {
            interactingObj = null;
        }
    }
}
