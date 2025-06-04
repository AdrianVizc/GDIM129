using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractSatellite : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject interactUI;
    private bool playerInRange;

    private void Start()
    {
        interactUI.SetActive(false);
    }

    private void Update()
    {
        if(playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            animator.SetBool("inRange", true);
            gameObject.GetComponent<Light>().color = Color.green;
            interactUI.SetActive(false);
            Destroy(gameObject.GetComponent<SphereCollider>());
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            playerInRange = true;
            interactUI.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            interactUI.SetActive(false);
        }
    }
}
