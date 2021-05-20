using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public LevelLoader levelLoader;

    public GameObject mainPage, levelSelectPage, difficultySelectPage1;

    public AudioSource buttonClickedSound, buttonHighlightedSound;

    // Start is called before the first frame update
    void Start()
    {
        mainPage.SetActive(true);
        levelSelectPage.SetActive(false);
        difficultySelectPage1.SetActive(false);

        Cursor.lockState = CursorLockMode.None;
    }

    // Update is called once per frame
    void Update()
    {
        
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
}
