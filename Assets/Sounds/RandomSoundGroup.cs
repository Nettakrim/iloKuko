using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class RandomSoundGroup : SoundGroup
{
    public AudioClip[] audioClips;
    public float octaveRange;
    public float volume = 1f;

    public override AudioClip GetAudioClip() {
        return audioClips[Random.Range(0, audioClips.Length)];
    }

    public override float GetPitch() {
        return Mathf.Pow(2, Random.Range(-octaveRange, octaveRange));
    }

    public override float GetVolume() {
        return volume;
    }
}
