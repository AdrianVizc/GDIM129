using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractSatellite : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject interactUI;
    [SerializeField] private AudioSource interactAudioSource; // Assign in Inspector
    private bool playerInRange;
    private bool isActivated = false;  // Track if satellite has been activated

    private void Start()
    {
        interactUI.SetActive(false);
    }

    private void Update()
    {
        if (!isActivated && playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            animator.SetBool("inRange", true);
            gameObject.GetComponent<Light>().color = Color.green;
            interactUI.SetActive(false);
            Destroy(gameObject.GetComponent<SphereCollider>());

            if (interactAudioSource != null)
            {
                interactAudioSource.Play();
            }

            isActivated = true;  // Prevent further activations
            playerInRange = false; // Optional: reset
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isActivated && other.CompareTag("Player"))
        {
            playerInRange = true;
            interactUI.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!isActivated && other.CompareTag("Player"))
        {
            playerInRange = false;
            interactUI.SetActive(false);
        }
    }
}
