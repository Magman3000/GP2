using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class DynamicSound : MonoBehaviour
{
    /*[SerializeField] AudioMixer audioMixer;
    [SerializeField] public string type;

    public void setVolume(float volume)
    {
        audioMixer.SetFloat(type, Mathf.Log10(volume) * 20);
    }*/ //Ref from volume slider script I previously made - Johan.

    [SerializeField] float volume, pitch, speed;//Placeholder for values from other sources.
    [SerializeField] float volumeMin, volumeMax;
    [SerializeField] float pitchMin, pitchMax;
    [SerializeField] float pitchMod, volumeMod;
    public void SpeedVolumeDynamics()
    {
        volume += speed * volumeMod;
        pitch += speed * pitchMod;


        if(volume < volumeMin) volume = volumeMin;
        else if(volume > volumeMax) volume = volumeMax;
        if(pitch < pitchMin) pitch = pitchMin;
        else if(pitch > pitchMax) pitch = pitchMax;

        //Update Pitch and volume
    }
    



}
