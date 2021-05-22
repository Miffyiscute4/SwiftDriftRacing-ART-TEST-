using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
    public Player_CarController car;
    public PauseMenu pauseMenu;

    public GameObject checkPointsObject;
    Transform[] checkPoints;
    bool[] ischeckPointNew = {false, true, true, true, true, true, true, true, true, true};

    public GameObject initialPopup;
    bool initialPopupShown;

    public GameObject popupsObject;
    public Transform[] popups;
    GameObject currentPopup;

    AudioSource[] carAudio;

    public AudioSource popupSound;

    // Start is called before the first frame update
    void Start()
    {
        checkPoints = checkPointsObject.GetComponentsInChildren<Transform>();

        //ischeckPointNew.Add(false);

        for (int i = 1; i < popups.Length; i++)
        {
            popups[i].gameObject.SetActive(false);
            //ischeckPointNew.Add(true);
        }

        initialPopup.SetActive(false);

        carAudio = car.GetComponents<AudioSource>();

    }

    void Update()
    {
        if (car.stopwatch_StartDelay >= car.startDelay && !initialPopupShown)
        {
            initialPopup.SetActive(true);

            car.enabled = false;
            pauseMenu.enabled = false;

            Time.timeScale = 0;
            Cursor.lockState = CursorLockMode.None;

            foreach (AudioSource a in carAudio)
            {
                a.Pause();
            }

            popupSound.Play();

            initialPopupShown = true;
        }
    }

    void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.tag == "CheckPoint")
        {
            DisplayPopup(other);
        }

    }

    public void ResumePopup()
    {
        car.enabled = true;
        pauseMenu.enabled = true;

        Time.timeScale = 1;

        if (!initialPopup.activeInHierarchy)
        {
            currentPopup.gameObject.SetActive(false);
        }
        else
        {
            initialPopup.SetActive(false);
        }

        Cursor.lockState = CursorLockMode.Locked;

        foreach (AudioSource a in carAudio)
        {
            a.UnPause();
        }
    }

    public void DisplayPopup(Collider other)
    {
        for (int i = 1; i < checkPoints.Length; i++)
        {
            if (other.gameObject == checkPoints[i].gameObject && ischeckPointNew[i])
            {
                car.enabled = false;
                pauseMenu.enabled = false;

                currentPopup = popups[i].gameObject;
                currentPopup.SetActive(true);

                Time.timeScale = 0;
                Cursor.lockState = CursorLockMode.None;

                foreach (AudioSource a in carAudio)
                {
                    a.Pause();
                }

                popupSound.Play();

                ischeckPointNew[i] = false;
            }
        }
    }
}
