using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractPopup : MonoBehaviour
{
    private Camera playerCam;

    private void Start()
    {
        playerCam = Camera.main;
    }

    private void Update()
    {
        transform.LookAt(playerCam.transform.position);
        transform.Rotate(0, 180, 0);
    }
}
