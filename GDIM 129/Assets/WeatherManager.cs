using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeatherManager : MonoBehaviour
{
    [SerializeField, Range(0f, 1f)] private float rainIntensity;
    [SerializeField, Range(0f, 1f)] private float snowIntensity;
    [SerializeField, Range(0f, 1f)] private float fogIntensity;
    [SerializeField, Range(0f, 1f)] private float hailIntensity;
}
