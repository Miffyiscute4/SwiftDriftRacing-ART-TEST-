using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelLoader : MonoBehaviour
{
    public GameObject loadingScreen;
    public Slider slider;
    public Text loadingText;
    AudioSource loadingMusic, endMusic;
    public AudioSource AudioSourceToDisable;

    public Player_CarController car;
    AudioSource[] carSounds;

    public UI ui;

    public bool isMenu = false, isEndScreen = false;

    void Start()
    {
        loadingMusic = GetComponent<AudioSource>();

        if (!isMenu)
        {
            carSounds = car.GetComponents<AudioSource>();
        }

        if (isEndScreen)
        {
            endMusic = ui.endMusic;
        }
    }

    public void LoadLevel(string sceneName)
    {
        StartCoroutine(LoadAsync(sceneName));
    }

    IEnumerator LoadAsync (string sceneName)
    {
        AudioSourceToDisable.Stop();

        if (!isMenu)
        {
            foreach (AudioSource a in carSounds)
            {
                a.Stop();
            }
        }

        if (endMusic != null)
        {
            endMusic.Stop();
        }
        

        loadingScreen.SetActive(true);
        loadingMusic.Play();

        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);

            Debug.Log((int) (progress * 100) + "%");

            loadingText.text = (int)(progress * 100) + "%";
            slider.value = progress;

            yield return null;
        }
    }
}
