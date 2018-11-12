using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {

    // Sound Effects
    public AudioClip errorSound;
    public AudioClip selectSound;

    // Background Music
    public AudioClip bgMusic1;
    public AudioClip bgMusic2;
    public AudioClip bgMusic3;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void PlaySound(string sound)
    {
        switch (sound)
        {
            case "ERROR":
                break;
            case "SELECT":
                break;
            default:
                Debug.LogErrorFormat("{0} isn't a valid string for sound selection", sound);
                break;
        }
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
