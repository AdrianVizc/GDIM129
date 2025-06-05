using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class HallwayEvent : MonoBehaviour
{
    [SerializeField] private GameObject mannequinHallway_half;
    [SerializeField] private GameObject mannequinHallway_end;
    [SerializeField] private AudioSource audioBreath;

    private bool firstEventFlag;

    // Start is called before the first frame update
    void Start()
    {
        firstEventFlag = false;
        GetComponent<BoxCollider>().center = new Vector3(12, 0, 0);
        mannequinHallway_half.SetActive(true);
        mannequinHallway_end.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player") && !firstEventFlag)
        {
            firstEventFlag = true;
            mannequinHallway_half.SetActive(false);
            mannequinHallway_end.SetActive(true);
            audioBreath.Play();

            GetComponent<BoxCollider>().center = new Vector3(-4, 0, 0);
        }
        else if(other.CompareTag("Player") && firstEventFlag)
        {
            GetComponent<BoxCollider>().enabled = false;
            mannequinHallway_end.SetActive(false);
            audioBreath.Play();
        }
    }
}
