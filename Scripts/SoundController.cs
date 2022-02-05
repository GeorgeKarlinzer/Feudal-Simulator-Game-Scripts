using System.Collections;
using UnityEngine;

public class SoundController : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip[] clips = new AudioClip[0];
    
    private void Awake()
    {
        StartCoroutine(PlayAudio(clips[0]));
    }

    IEnumerator PlayAudio(AudioClip clip)
    {
        audioSource.clip = clip;
        audioSource.Play();
        yield return new WaitForSeconds(audioSource.clip.length);
        if(audioSource.clip == clip)
            FinishPlaySound();
    }

    private void FinishPlaySound()
    {
        StartCoroutine(PlayAudio(clips[Random.Range(0, clips.Length)]));        
    }
}
