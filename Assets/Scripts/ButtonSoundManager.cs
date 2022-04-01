using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class ButtonSoundManager : MonoBehaviour
{
    public AudioClip[] audioClips;
    public AudioSource audio;
    private void Start()
    {
        audio = GetComponent<AudioSource>();
    }
    public void ClickButtonSound()
    {
        audio.PlayOneShot(audioClips[0]);
    }
    public void SelectButtonSound()
    {
        audio.PlayOneShot(audioClips[1]);
    }
}
