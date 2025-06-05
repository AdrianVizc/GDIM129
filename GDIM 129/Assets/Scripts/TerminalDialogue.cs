using System.Collections;
using UnityEngine;
using TMPro;

public class TerminalDialogue : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private TextMeshProUGUI textComponent;

    [TextArea(2, 5)]
    [SerializeField] private string[] dialogueLines;

    [SerializeField] private float textDisplaySpeed = 0.05f;

    [Header("Dialogue State")]
    private int index = 0;

    [Header("References")]
    [SerializeField] private PlayerMovement playerObject;
    [SerializeField] private PlayerCam cameraObject;

    private PauseMenu pauseMenuObject;

    [Header("Audio")]
    [SerializeField] private AudioSource startAudioSource;
    [SerializeField] private AudioSource endAudioSource;
    [SerializeField] private float fadeDuration = 1.0f;

    [Range(0f, 1f)]
    [SerializeField] private float startAudioMaxVolume = 0.3f; // Volume the audio fades in to

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

        if (startAudioSource != null)
        {
            startAudioSource.volume = 0f;
            startAudioSource.Play();
            // Fade in to a quieter volume instead of 1f
            StartCoroutine(FadeAudio(startAudioSource, startAudioMaxVolume, fadeDuration));
        }

        StartDialogue();
    }

    private void Update()
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
        if (dialogueLines.Length > 0 && gameObject.activeSelf)
        {
            StartCoroutine(TypeLine());
        }
    }

    private IEnumerator TypeLine()
    {
        textComponent.text = string.Empty;
        foreach (char c in dialogueLines[index])
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
            if (endAudioSource != null)
            {
                endAudioSource.volume = 1f;
                endAudioSource.Play();
                StartCoroutine(FadeAudio(endAudioSource, 0f, fadeDuration));
            }

            playerObject.isDialogueOn = false;
            cameraObject.isDialogueOn = false;

            StartCoroutine(DeactivateAfterSound(fadeDuration));
        }
    }

    private IEnumerator DeactivateAfterSound(float delay)
    {
        yield return new WaitForSeconds(delay);
        gameObject.SetActive(false);
    }

    private IEnumerator FadeAudio(AudioSource source, float targetVolume, float duration)
    {
        float startVolume = source.volume;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            source.volume = Mathf.Lerp(startVolume, targetVolume, time / duration);
            yield return null;
        }

        source.volume = targetVolume;

        if (targetVolume == 0f)
            source.Stop();
    }
}
