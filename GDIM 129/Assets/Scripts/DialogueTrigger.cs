using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour, IInteractable
{
    [SerializeField] private GameObject dialogueHolder;

    public void Interact()
    {
        dialogueHolder.SetActive(true);
    }
}
