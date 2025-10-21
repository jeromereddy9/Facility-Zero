using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseMenuScript : MonoBehaviour
{
    [Header("UI Elements")]
    public static bool paused = false;

    [Header("level To Load")]
    public string _retry;

    [SerializeField] private GameObject PauseCanvas;
    [SerializeField] private GameObject Canvas;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        PauseCanvas.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current.pKey.wasReleasedThisFrame)
        {
            if (!paused)
            //{
            //    Play();
            //}
            //else 
            {
                Stop();
            }
        }
    }

    void Stop()
    {
        PauseCanvas.SetActive(true);
        Time.timeScale = 0.0f;
        paused = true;
        Canvas.SetActive(false);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void Play()
    {
        PauseCanvas.SetActive(false);
        Time.timeScale = 1.0f;
        paused = false;
        Canvas.SetActive(true);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void MainMenuButton()
    {
        Time.timeScale = 1.0f;
        paused = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex-1);
    }

    public void RetryGame()
    {
        Time.timeScale = 1.0f;
        paused = false;
        SceneManager.LoadScene(_retry);
    }
    public void Quit()
    {
        Debug.Log("Quitting Game...");
        Application.Quit();
    }
}
