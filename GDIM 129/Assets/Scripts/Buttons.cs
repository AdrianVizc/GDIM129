using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Buttons : MonoBehaviour
{
    [SerializeField] private string gameScene;

    public void StartButton()
    {
        SceneManager.LoadScene(gameScene);
    }

    public void QuitButton()
    {
        Application.Quit();
    }
}
