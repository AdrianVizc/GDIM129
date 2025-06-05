using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Dialogue : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textComponent;
    [SerializeField] private string[] dialogueLines;
    [SerializeField] private float textDisplaySpeed;
    [SerializeField] private int index;

    [Space]
    [SerializeField] private PlayerMovement playerObject;
    [SerializeField] private PlayerCam cameraObject;

    private PauseMenu pauseMenuObject;

    private void Start()
    {
        pauseMenuObject = GameObject.Find("PauseMenuCanvas").GetComponent<PauseMenu>();
    }

    private void OnEnable()
    {
        index = 0;
        textComponent.text = string.Empty;

        playerObject.isDialogueOn = true;
        cameraObject.isDialogueOn = true;

        StartDialogue();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !pauseMenuObject.isPaused)
        {
            if (textComponent.text == dialogueLines[index])
            {
                NextLine();
            }
            else
            {
                StopAllCoroutines();
                textComponent.text = dialogueLines[index];
            }
        }
    }

    private void StartDialogue()
    {
        index = 0;
        if (this.gameObject.activeSelf)
        {
            StartCoroutine(TypeLine());
        }
    }

    private IEnumerator TypeLine()
    {
        foreach (char c in dialogueLines[index].ToCharArray())
        {
            textComponent.text += c;
            yield return new WaitForSeconds(textDisplaySpeed);
        }
    }

    private void NextLine()
    {
        if (index < dialogueLines.Length - 1)
        {
            index++;
            textComponent.text = string.Empty;
            StartCoroutine(TypeLine());
        }
        else
        {
            playerObject.isDialogueOn = false;
            cameraObject.isDialogueOn = false;
            gameObject.SetActive(false);            
        }
    }
}
