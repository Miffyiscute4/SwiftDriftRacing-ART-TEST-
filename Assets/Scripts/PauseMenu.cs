using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    bool isPaused = false;
    public GameObject pauseMenu;

    public Player_CarController carPlayer;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        pauseMenu.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!isPaused)
            {
                PauseGame();
            }
            else
            {
                ResumeGame();
            }
        }


    }

    public void PauseGame()
    {
        carPlayer.enabled = false;

        pauseMenu.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Time.timeScale = 0;

        isPaused = true;

        Debug.Log("Paused");
    }

    public void ResumeGame()
    {
        carPlayer.enabled = true;

        pauseMenu.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Time.timeScale = 1;

        isPaused = false;

        Debug.Log("Resuming");
    }

    public void ExitGame()
    {
        SceneManager.LoadScene("MainMenu");
    }


}
