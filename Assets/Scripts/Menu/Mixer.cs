using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class Mixer : MonoBehaviour
{

    public AudioSource Musicsource;

    public void SetMusiVolume(float volume)
    {
        Musicsource.volume = volume;
    }
  

    //public void ToggleAudioVolume()
    //{
    //    AudioListener.volume = AudioListener.volume == 0 ? 1 : 0;  //사운드 끄기
    //}

}
