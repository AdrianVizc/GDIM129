using UnityEngine;

public class FogZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GlobalFogManager.Instance?.EnterFogZone();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GlobalFogManager.Instance?.ExitFogZone();
        }
    }
}
