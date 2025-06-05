using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    private bool isPaused;
    private CanvasGroup canvasGroup;

    private void Start()
    {
        isPaused = false;

        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

    private void Update()
    {
        EscapeButtonHandler();
    }

    private void EscapeButtonHandler()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!isPaused)
            {
                isPaused = true;
                PauseGame(isPaused);
                ShowPauseMenu(isPaused);

                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else 
            {
                isPaused = false;
                PauseGame(isPaused);
                ShowPauseMenu(isPaused);

                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
    }

    private void PauseGame(bool pause)
    {
        Time.timeScale = Convert.ToInt32(!pause);
    }

    private void ShowPauseMenu(bool show)
    {
        canvasGroup.alpha = Convert.ToInt32(show);
        canvasGroup.interactable = show;
        canvasGroup.blocksRaycasts = show;
    }
}
