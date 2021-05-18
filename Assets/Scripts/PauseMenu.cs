using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    bool isPaused = false;
    public GameObject pauseMenu;

    public Player_CarController carPlayer;

    public LevelLoader levelLoader;

    public AudioSource music, pauseMusic, pauseSound, buttonHighlightedSound, buttonClickedSound;

    public AudioSource[] carSounds;



    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        pauseMenu.SetActive(false);

        music.Play();

        carSounds = carPlayer.GetComponents<AudioSource>();
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

        music.Pause();
        pauseMusic.Play();
        pauseSound.Play();
        
        foreach (AudioSource a in carSounds)
        {
            a.Pause();
        }

        isPaused = true;

        Debug.Log("Paused");
    }

    public void ResumeGame()
    {
        carPlayer.enabled = true;

        pauseMenu.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Time.timeScale = 1;

        music.UnPause();
        pauseMusic.Stop();
        pauseSound.Play();

        foreach (AudioSource a in carSounds)
        {
            a.UnPause();
        }

        isPaused = false;

        Debug.Log("Resuming");
    }

    public void ExitGame()
    {
        ResumeGame();
        levelLoader.LoadLevel("MainMenu");
    }

    public void PlayHighlightedSound()
    {
        buttonHighlightedSound.Play();
    }

    public void PlayClickedSound()
    {
        buttonClickedSound.Play();
    }

}
