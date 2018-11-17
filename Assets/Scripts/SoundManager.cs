using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {

    public AudioSource source;

    // Sound Effects
    public AudioClip errorClip;
    public AudioClip selectClip;

    // Background Music
    public AudioClip bgMusic1;
    public AudioClip bgMusic2;
    public AudioClip bgMusic3;

    public void PlaySound(string sound)
    {
        switch (sound)
        {
            case "ERROR":
                source.clip = errorClip;
                break;
            case "SELECT":
                source.clip = selectClip;
                break;
            default:
                Debug.LogErrorFormat("{0} isn't a valid string for sound selection", sound);
                break;
        }
        if (!source.isPlaying)
            source.Play();
    }

    public void PlayMusic(int music)
    {
        switch (music)
        {
            case 1:
                break;
            case 2:
                break;
            case 3:
                break;
            default:
                Debug.LogErrorFormat("{0} isn't a valid int for music selection", music);
                break;

        }
    }
}
