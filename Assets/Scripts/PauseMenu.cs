using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    bool isPaused = false;
    public GameObject pauseMenu, settingsPage;

    public Player_CarController carPlayer;

    public LevelLoader levelLoader;

    public AudioSource music, pauseMusic, pauseSound, buttonHighlightedSound, buttonClickedSound;

    public AudioSource[] carSounds;

    public AudioMixer audioMixer;

    //settings menu
    public Slider volumeSlider;
    public Dropdown graphicsDropDown;


    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        pauseMenu.SetActive(false);

        music.Play();

        carSounds = carPlayer.GetComponents<AudioSource>();

        //sets volume visuals to current volume settings
        float volume = 0;

        audioMixer.GetFloat("MainVolume", out volume);

        Debug.Log(volume);
        volumeSlider.value = volume;

        //sets graphics visuals to current graphics settings
        int quality;

        quality = QualitySettings.GetQualityLevel();

        Debug.Log(quality);
        graphicsDropDown.value = quality;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) && !settingsPage.activeInHierarchy)
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

    public void OpenSettingsMenu()
    {
        settingsPage.SetActive(true);
        pauseMenu.SetActive(false);
    }

    public void BackButtonSettingsMenu()
    {
        settingsPage.SetActive(false);
        pauseMenu.SetActive(true);
    }

    public void ChangeAudio(float volume)
    {
        audioMixer.SetFloat("MainVolume", volume);
        Debug.Log(volume);
    }

    public void ChangeGraphics(int Quality)
    {
        QualitySettings.SetQualityLevel(Quality);
    }
}
