using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractSatellite : MonoBehaviour
{
    [SerializeField] private Animator animator;
    private bool playerInRange;

    private void Update()
    {
        if(playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            animator.SetBool("inRange", true);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }
}
