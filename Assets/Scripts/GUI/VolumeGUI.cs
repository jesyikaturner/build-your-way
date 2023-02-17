using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VolumeGUI : MonoBehaviour {

    public SoundManager sm;

    public Slider soundSlider;
    public Slider musicSlider;

    // Use this for initialization
    void Start () {
        // https://answers.unity.com/questions/1111955/how-do-i-make-a-change-a-volume-with-a-slider.html
        soundSlider.value = sm.soundSource.volume;
        musicSlider.value = sm.musicSource.volume;
    }

    public void SetSoundVolume()
    {
        sm.SetVolume("SoundEffects", soundSlider.maxValue,soundSlider.value);
        sm.PlaySound("SELECT");
    }

    public void SetMusicVolume()
    {
        sm.SetVolume("Music", musicSlider.maxValue, musicSlider.value);
        sm.PlaySound("SELECT");
    }
}
