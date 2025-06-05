using UnityEngine;
using System.Collections;

public class GlobalFogManager : MonoBehaviour
{
    public static GlobalFogManager Instance;

    [Header("Fog Settings")]
    public float indoorFogDensity = 0.002f;
    public float transitionDuration = 1f;

    [Header("Wind Audio Settings")]
    [SerializeField] private AudioSource windAudioSource;
    [SerializeField] private AudioLowPassFilter windLowPassFilter;
    [SerializeField] private float outdoorVolume = 0.8f;
    [SerializeField] private float indoorVolume = 0.2f;
    [SerializeField] private float outdoorCutoff = 22000f;
    [SerializeField] private float indoorCutoff = 800f;

    private float originalFogDensity;
    private int fogZoneCounter = 0;
    private Coroutine fogCoroutine;
    private Coroutine audioCoroutine;
    private Coroutine filterCoroutine;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        originalFogDensity = RenderSettings.fogDensity;

        if (windAudioSource != null && !windAudioSource.isPlaying)
            windAudioSource.Play();
    }

    public void EnterFogZone()
    {
        fogZoneCounter++;
        if (fogZoneCounter == 1)
        {
            StartFogTransition(indoorFogDensity);
            StartWindTransition(indoorVolume, indoorCutoff);
        }
    }

    public void ExitFogZone()
    {
        fogZoneCounter = Mathf.Max(0, fogZoneCounter - 1);
        if (fogZoneCounter == 0)
        {
            StartFogTransition(originalFogDensity);
            StartWindTransition(outdoorVolume, outdoorCutoff);
        }
    }

    private void StartFogTransition(float targetDensity)
    {
        if (fogCoroutine != null)
            StopCoroutine(fogCoroutine);

        fogCoroutine = StartCoroutine(FogTransition(RenderSettings.fogDensity, targetDensity));
    }

    private IEnumerator FogTransition(float start, float end)
    {
        float elapsed = 0f;
        while (elapsed < transitionDuration)
        {
            elapsed += Time.deltaTime;
            RenderSettings.fogDensity = Mathf.Lerp(start, end, elapsed / transitionDuration);
            yield return null;
        }
        RenderSettings.fogDensity = end;
    }

    private void StartWindTransition(float targetVolume, float targetCutoff)
    {
        if (audioCoroutine != null)
            StopCoroutine(audioCoroutine);
        if (filterCoroutine != null)
            StopCoroutine(filterCoroutine);

        audioCoroutine = StartCoroutine(FadeAudioVolume(targetVolume));
        filterCoroutine = StartCoroutine(FadeLowPassFilter(targetCutoff));
    }

    private IEnumerator FadeAudioVolume(float target)
    {
        float start = windAudioSource.volume;
        float time = 0f;

        while (time < transitionDuration)
        {
            time += Time.deltaTime;
            windAudioSource.volume = Mathf.Lerp(start, target, time / transitionDuration);
            yield return null;
        }

        windAudioSource.volume = target;
    }

    private IEnumerator FadeLowPassFilter(float target)
    {
        float start = windLowPassFilter.cutoffFrequency;
        float time = 0f;

        while (time < transitionDuration)
        {
            time += Time.deltaTime;
            windLowPassFilter.cutoffFrequency = Mathf.Lerp(start, target, time / transitionDuration);
            yield return null;
        }

        windLowPassFilter.cutoffFrequency = target;
    }
}
