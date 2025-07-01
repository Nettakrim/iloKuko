using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DropOff : MonoBehaviour
{
    [SerializeField] private BoxCollider2D bounds;

    private Item item = null;
    private Box box = null;

    private bool isOpen;
    private float current;
    [SerializeField] private float rounding;
    [SerializeField] private float target;
    private float inPos;
    [SerializeField] private float speed;

    [SerializeField] private SoundGroup open;
    [SerializeField] private SoundGroup close;
    [SerializeField] private SoundGroup accept;
    [SerializeField] private SoundGroup reject;

    private AudioSource sound;

    void Start()
    {
        inPos = transform.position.x;
        target -= inPos;
        float offset = target % rounding;
        target += offset + rounding;
        inPos += offset;

        sound = AudioManager.instance.DedicatedClaim();
    }

    void OnDestroy()
    {
        if (sound)
        {
            AudioManager.instance.DedicatedRelease(sound);
        }
    }

    void Update()
    {
        current = Mathf.MoveTowards(current, isOpen ? target : 0, Time.deltaTime * speed);
        if (item)
        {
            if (current == 0)
            {
                box.OnSubmit(item);
                if (!isOpen)
                {
                    Destroy(item.gameObject);
                    accept.Play(true);
                }
                else
                {
                    reject.Play(true);
                }
            }
            if (current == target)
            {
                item.transform.SetParent(box.transform);
                box.ItemRejected(item);
                item = null;
            }
        }
        transform.position = new Vector3(inPos + Mathf.Floor(current / rounding) * rounding, transform.position.y, transform.position.z);
    }

    public void Submit(Item item, Box box)
    {
        this.item = item;
        this.box = box;
        item.transform.parent = transform;
        isOpen = false;
        PlayTraySound();
    }

    public bool IsHovered(Vector2 mousePos)
    {
        if (!isOpen || item || Global.submissionDisabled)
        {
            return false;
        }

        Bounds bounds = this.bounds.bounds;
        return mousePos.x >= bounds.min.x && mousePos.y >= bounds.min.y && mousePos.x <= bounds.max.x && mousePos.y <= bounds.max.y;
    }

    public void SetOpen(bool to)
    {
        if (isOpen == to || item)
        {
            return;
        }
        if (Global.submissionDisabled)
        {
            if (isOpen)
            {
                isOpen = false;
                PlayTraySound();
            }
            return;
        }
        isOpen = to;
        PlayTraySound();
    }

    private void PlayTraySound()
    {
        float t = 0;
        if (sound.isPlaying)
        {
            t = Mathf.Clamp(0.7f - sound.time, 0, 0.55f);
            sound.Stop();
        }

        SoundGroup soundGroup = isOpen ? open : close;
        sound.clip = soundGroup.GetAudioClip();
        sound.pitch = soundGroup.GetPitch();
        sound.volume = soundGroup.GetVolume();
        sound.time = t;
        sound.Play();
    }

    public void Reject()
    {
        isOpen = true;
        PlayTraySound();
    }
}
