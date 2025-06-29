using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SoundGroup : ScriptableObject
{
    public abstract AudioClip GetAudioClip();

    public abstract float GetPitch();

    public abstract float GetVolume();

    public void Play(bool force) {
        AudioManager.instance.PlaySound(this, force);
    }
}
