using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
    public Player_CarController car;
    public PauseMenu pauseMenu;

    public GameObject checkPointsObject;
    Transform[] checkPoints;

    public GameObject popupsObject;
    public Transform[] popups;
    GameObject currentPopup;

    AudioSource[] carAudio;

    // Start is called before the first frame update
    void Start()
    {
        checkPoints = checkPointsObject.GetComponentsInChildren<Transform>();

        for (int i = 1; i < popups.Length; i++)
        {
            popups[i].gameObject.SetActive(false);
        }

        carAudio = car.GetComponents<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {

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
        currentPopup.gameObject.SetActive(false);
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
            if (other.gameObject == checkPoints[i].gameObject)
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
            }
        }
    }
}
