using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioVolumeHandler : MonoBehaviour
{

    public AudioMixer masterMixer;

    public void SetMusicVolume(float soundLevel)
    {
        masterMixer.SetFloat("MusicVolume", Mathf.Log(soundLevel) * 20);
    }
    public void SetGlobalVolume(float soundLevel)
    {
        masterMixer.SetFloat("MasterVolume", Mathf.Log(soundLevel) * 20);
    }
    public void SetAmbianceVolume(float soundLevel)
    {
        masterMixer.SetFloat("AmbianceVolume", Mathf.Log(soundLevel) * 20);
    }
    public void SetUIVolume(float soundLevel)
    {
        masterMixer.SetFloat("UIVolume", Mathf.Log(soundLevel) * 20);
    }
}
