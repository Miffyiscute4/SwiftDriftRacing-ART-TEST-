using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public LevelLoader levelLoader;

    public GameObject mainPage, levelSelectPage, difficultySelectPage1, settingsPage;

    public AudioSource buttonClickedSound, buttonHighlightedSound;

    public AudioMixer audioMixer;

    //settings menu
    public Slider volumeSlider;
    public Dropdown graphicsDropDown;



    // Start is called before the first frame update
    void Start()
    {
        mainPage.SetActive(true);
        levelSelectPage.SetActive(false);
        difficultySelectPage1.SetActive(false);
        settingsPage.SetActive(false);

        Cursor.lockState = CursorLockMode.None;


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

        if (!Screen.fullScreen)
        {
            Screen.fullScreen = true;
        }
    }

    public void LevelSelect()
    {
        mainPage.SetActive(false);
        levelSelectPage.SetActive(true);
    }

    public void DifficultySelect()
    {
        levelSelectPage.SetActive(false);
        difficultySelectPage1.SetActive(true);
    }

    public void QuitGame()
    {
        Debug.Log("Quitting Game");

        Application.Quit();
    }

    public void LoadTrack(string trackName)
    {
        levelSelectPage.SetActive(false);
        levelLoader.LoadLevel(trackName);
    }

    public void PlayHighlightedSound()
    {
        buttonHighlightedSound.Play();
    }

    public void PlayClickedSound()
    {
        buttonClickedSound.Play();
    }

    public void BackButtonDifficultySelect()
    {
        difficultySelectPage1.SetActive(false);
        levelSelectPage.SetActive(true);
    }

    public void BackButtonLevelSelect()
    {
        levelSelectPage.SetActive(false);
        mainPage.SetActive(true);
    }

    public void PlayBounceAnimation(Animator anim)
    {
        anim.enabled = !anim.enabled;

        if (anim.enabled)
        {
            anim.transform.localScale *= 1.25f;
        }
        else
        {
            anim.transform.localScale /= 1.25f;
        }
    }

    public void OpenSettingsMenu()
    {
        settingsPage.SetActive(true);
        mainPage.SetActive(false);
    }

    public void BackButtonSettingsMenu()
    {
        settingsPage.SetActive(false);
        mainPage.SetActive(true);
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
