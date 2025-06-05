using UnityEngine;

public class PlayerFogToggleZone : MonoBehaviour
{
    [Tooltip("Fog density to use indoors.")]
    public float indoorFogDensity = 0.002f;

    [Tooltip("How fast the fog transitions.")]
    public float transitionDuration = 1.0f;

    private float originalFogDensity;
    private Coroutine transitionCoroutine;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            originalFogDensity = RenderSettings.fogDensity;
            StartFogTransition(indoorFogDensity);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartFogTransition(originalFogDensity);
        }
    }

    private void StartFogTransition(float targetDensity)
    {
        if (transitionCoroutine != null)
        {
            StopCoroutine(transitionCoroutine);
        }
        transitionCoroutine = StartCoroutine(FogTransition(RenderSettings.fogDensity, targetDensity, transitionDuration));
    }

    private System.Collections.IEnumerator FogTransition(float start, float end, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            RenderSettings.fogDensity = Mathf.Lerp(start, end, elapsed / duration);
            yield return null;
        }
        RenderSettings.fogDensity = end;
    }
}
