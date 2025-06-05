using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Generator : MonoBehaviour, IInteractable
{
    [Header("GameObjects")]
    [SerializeField] private GameObject dialogueHolder;
    [SerializeField] private GameObject lightHolder;
    [SerializeField] private GameObject interactHolder;

    [Header("Audio")]
    [SerializeField] private AudioSource activationAudio;

    public void Interact()
    {
        dialogueHolder.SetActive(true);
        lightHolder.SetActive(true);
        interactHolder.SetActive(false);

        if (activationAudio != null && !activationAudio.isPlaying)
        {
            activationAudio.Play();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            dialogueHolder.SetActive(true);
        }
    }
}
