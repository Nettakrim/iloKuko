using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    public AudioSource audioSourcePrefab;

    public int sourceCount;

    private AudioSource[] audioSources;

    public AudioMixer audioMixer;

    void Start() {
        if (instance != null) {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(this);

        audioSources = new AudioSource[sourceCount];
        for (int i = 0; i < sourceCount; i++) {
            audioSources[i] = Instantiate(audioSourcePrefab, transform);
        }
    }

    public void PlaySound(SoundGroup soundGroup, bool force) {
        if (soundGroup == null)
        {
            return;
        }
        
        AudioSource audioSource = GetAvailableAudioSource(force);
        
        if (audioSource != null) {
            audioSource.clip = soundGroup.GetAudioClip();
            audioSource.pitch = soundGroup.GetPitch();
            audioSource.volume = soundGroup.GetVolume();
            
            audioSource.Play();
        }
    }

    private AudioSource GetAvailableAudioSource(bool force) {
        for (int i = 0; i < sourceCount; i++) {
            AudioSource audioSource = audioSources[i];
            if (!audioSource.isPlaying) {
                return audioSource;
            }
        }

        // if a sound needs to play no mater what, find the one that is closest to finishing
        if (!force) return null;

        AudioSource currentSource = audioSources[0];
        float currentPercent = currentSource.timeSamples/(float)currentSource.clip.samples;

        for (int i = 1; i < sourceCount; i++) {
            AudioSource audioSource = audioSources[i];
            float percent = audioSource.timeSamples/(float)audioSource.clip.samples;
            if (percent > currentPercent) {
                currentSource = audioSource;
                currentPercent = percent;
            }
        }

        return currentSource;
    }

    public void SetVolume(string name, float volume) {
        float db = Mathf.Log(Mathf.Max(volume, 0.001f)) * 20;
        audioMixer.SetFloat(name, db);
    }
}
