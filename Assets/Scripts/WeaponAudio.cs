using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponAudio : MonoBehaviour
{
    public AudioSource audioSource;

    [Space(20)]
    [SerializeField] private AudioClip[] fireAudioArray;
    [SerializeField] private AudioClip drawAudio;
    [SerializeField] private AudioClip reloadClipOutAudio;
    [SerializeField] private AudioClip reloadClipInAudio;
    [SerializeField] private AudioClip reloadSlideInAudio;

    public void PlayFireAudio()
    {
        audioSource.pitch = Random.Range(0.9f, 1.1f);
        audioSource.PlayOneShot(fireAudioArray[Random.Range(0, fireAudioArray.Length)]);
    }
    public void PlayDrawAudio()
    {
        audioSource.pitch = Random.Range(0.9f, 1.1f);
        audioSource.PlayOneShot(drawAudio);
    }
    public void PlayClipOutAudio()
    {
        audioSource.pitch = Random.Range(0.9f, 1.1f);
        audioSource.PlayOneShot(reloadClipOutAudio);
    }
    public void PlayClipInAudio()
    {
        audioSource.pitch = Random.Range(0.9f, 1.1f);
        audioSource.PlayOneShot(reloadClipInAudio);
    }
    public void PlaySlideInAudio()
    {
        audioSource.pitch = Random.Range(0.9f, 1.1f);
        audioSource.PlayOneShot(reloadSlideInAudio);
    }
}
