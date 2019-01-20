using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour {

    public AudioSource soundSource;
    public AudioSource musicSource;

    public AudioMixer mixer;

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
                soundSource.clip = errorClip;
                break;
            case "SELECT":
                soundSource.clip = selectClip;
                break;
            default:
                Debug.LogErrorFormat("{0} isn't a valid string for sound selection", sound);
                break;
        }
        if (!soundSource.isPlaying)
            soundSource.Play();
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

    public void SetVolume(string name, float maxValue, float value)
    {
        mixer.SetFloat(name, ConvertToDecibel(value / maxValue));
    }

    private float ConvertToDecibel(float value)
    {
        return Mathf.Log10(Mathf.Max(value, 0.0001f)) * 20f;
    }
}
