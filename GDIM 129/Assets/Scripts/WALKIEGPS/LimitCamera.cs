using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LimitCamera : MonoBehaviour
{   //Objects
    public GameObject Player;
    public Camera minimapCamera;
    //Camera Lag
    public float updateInterval = 25f;
    private float _timer = 0f;

    private void LateUpdate()
    {
        transform.position = new Vector3(Player.transform.position.x, 40f, Player.transform.position.z);
        float playerYRotation = Player.transform.eulerAngles.y;
        transform.eulerAngles = new Vector3(90f, playerYRotation, 0f);
    }
    private float nextUpdateTime;

    void Start()
    {
        minimapCamera.enabled = false;
        nextUpdateTime = Random.Range(0.2f, 0.4f);
    }

    void Update()
    {
        _timer += Time.deltaTime;

        if (_timer >= nextUpdateTime)
        {
            minimapCamera.Render();
            _timer = 0f;
            nextUpdateTime = Random.Range(0.2f, 0.4f); // simulate irregular GPS pings
        }
    }
}

