using UnityEngine;

[RequireComponent(typeof(Light))]
public class LightFlicker : MonoBehaviour
{
    public float minIntensity = 0.5f;
    public float maxIntensity = 1.5f;
    public float flickerSpeed = 0.1f;

    private Light _light;

    void Start()
    {
        _light = GetComponent<Light>();
        InvokeRepeating(nameof(Flicker), 0f, flickerSpeed);
    }

    void Flicker()
    {
        _light.intensity = Random.Range(minIntensity, maxIntensity);
    }
}