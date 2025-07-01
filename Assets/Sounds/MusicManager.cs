using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager instance;

    private bool musicSource;
    public AudioSource musicA;
    public AudioSource musicB;
    public float musicFadeSpeed;

    void Start()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(this);
    }

    public void PlayMusic(AudioClip audioClip)
    {
        musicSource = !musicSource;

        AudioSource music = musicSource ? musicA : musicB;
        AudioSource other = musicSource ? musicB : musicA;
        music.clip = audioClip;

        if (other.clip == null)
        {
            music.volume = 1;
            other.volume = 0;
        }
        else
        {
            music.volume = 0;
        }

        music.Play();

        if (audioClip != null)
        {
            music.timeSamples = other.timeSamples;
        }
    }
    
    private void Update()
    {
        AudioSource musicFade = musicSource ? musicB : musicA;
        if (musicFade.volume > 0)
        {
            AudioSource other = musicSource ? musicA : musicB;
            musicFade.volume -= Time.deltaTime * (other.clip == null ? 5 : musicFadeSpeed);

            other.volume = 1 - musicFade.volume;
            if (other.volume == 1)
            {
                musicFade.Stop();
            }
        }
    }
}
