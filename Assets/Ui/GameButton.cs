using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class GameButton : MonoBehaviour
{
    [SerializeField] private bool hideWebgl;
    [SerializeField] private int ignoreMask = 3;

    [SerializeField] private UnityEvent onPress;

    private bool pressed;
    private bool lastHover;

    [SerializeField] private SoundGroup hoverOn;
    [SerializeField] private SoundGroup hoverOff;
    [SerializeField] private SoundGroup clickDown;
    [SerializeField] private SoundGroup clickUp;

    #if UNITY_WEBGL
    void Start()
    {
        if (hideWebgl) {
            Destroy(gameObject);
        }
    }
    #endif


    void Update()
    {
        bool up = Input.GetKeyUp(KeyCode.Mouse0);
        bool hover = IsMouseOver();

        if (hover)
        {
            UpdateButton((pressed && Input.GetKey(KeyCode.Mouse0)) ? 2 : 1);
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                pressed = true;
                AudioManager.instance.PlaySound(clickDown, false);
            }
            if (up && pressed)
            {
                onPress.Invoke();
                AudioManager.instance.PlaySound(clickUp, false);
            }
        }
        else
        {
            UpdateButton(0);
        }

        if (up)
        {
            pressed = false;
        }

        if (hover != lastHover)
        {
            AudioManager.instance.PlaySound(hover ? hoverOn : hoverOff, false);
            lastHover = hover;
        }
    }

    protected virtual bool IsMouseOver()
    {
        return Global.MouseOver(transform as RectTransform, ignoreMask);
    }

    protected abstract void UpdateButton(int state);
}
