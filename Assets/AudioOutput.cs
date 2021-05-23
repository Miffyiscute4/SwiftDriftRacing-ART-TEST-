using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioOutput : MonoBehaviour
{
    public AudioMixerGroup audioMixer;

    // Start is called before the first frame update
    void Start()
    {
        AudioSource[] audioSources = FindObjectsOfType<AudioSource>();

        foreach (AudioSource a in audioSources)
        {
            a.outputAudioMixerGroup = audioMixer;
        }
    }
}
