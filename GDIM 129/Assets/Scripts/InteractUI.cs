using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractUI : MonoBehaviour
{
    private Camera playerCam;

    private void Start()
    {
        playerCam = Camera.main;
    }

    private void Update()
    {
        transform.LookAt(playerCam.transform.position);
    }
}
