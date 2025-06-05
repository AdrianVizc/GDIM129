using UnityEngine;
using System.Collections;

public class GlobalFogManager : MonoBehaviour
{
    public static GlobalFogManager Instance;

    public float indoorFogDensity = 0.002f;
    public float transitionDuration = 1f;

    private float originalFogDensity;
    private int fogZoneCounter = 0;
    private Coroutine transitionCoroutine;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        originalFogDensity = RenderSettings.fogDensity;
    }

    public void EnterFogZone()
    {
        fogZoneCounter++;
        if (fogZoneCounter == 1)
        {
            StartFogTransition(indoorFogDensity);
        }
    }

    public void ExitFogZone()
    {
        fogZoneCounter = Mathf.Max(0, fogZoneCounter - 1);
        if (fogZoneCounter == 0)
        {
            StartFogTransition(originalFogDensity);
        }
    }

    private void StartFogTransition(float targetDensity)
    {
        if (transitionCoroutine != null)
            StopCoroutine(transitionCoroutine);

        transitionCoroutine = StartCoroutine(FogTransition(RenderSettings.fogDensity, targetDensity));
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
}