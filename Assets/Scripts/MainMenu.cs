using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject mainPage, levelSelectPage;

    // Start is called before the first frame update
    void Start()
    {
        mainPage.SetActive(true);
        levelSelectPage.SetActive(false);
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

    public void QuitGame()
    {
        Application.Quit();

        Debug.Log("Quitting Game");
    }

    public void LoadTrack(string trackName)
    {
        SceneManager.LoadScene(trackName);
    }
}
