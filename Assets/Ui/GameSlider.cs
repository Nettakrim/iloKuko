using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameSlider : MonoBehaviour
{
    [SerializeField] private int ignoreMask = 3;

    private bool pressed;
    private bool lastHover;

    [SerializeField] private SoundGroup hoverOn;
    [SerializeField] private SoundGroup hoverOff;
    [SerializeField] private SoundGroup clickDown;
    [SerializeField] private SoundGroup clickUp;


    [SerializeField] private Slider slider;
    [SerializeField] private RectTransform handle;

    [SerializeField] private RectTransform sliderRegion;

    [SerializeField] private Graphic graphic;
    [SerializeField] private Color hover;
    [SerializeField] private Color press;


    void Update()
    {
        bool up = Input.GetKeyUp(KeyCode.Mouse0);
        bool hover = Global.MouseOver(sliderRegion, ignoreMask);

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
                AudioManager.instance.PlaySound(clickUp, false);
            }
        }
        else
        {
            UpdateButton(pressed ? 2 : 0);
        }

        if (pressed)
        {
            Vector2 pos = Global.GetMousePos();
            slider.value = (pos.x - sliderRegion.position.x - handle.sizeDelta.x / 2) / (sliderRegion.sizeDelta.x - handle.sizeDelta.x);
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

    private void UpdateButton(int state)
    {
        graphic.color = state == 0 ? Color.white : (state == 1 ? hover : press);
    }

    public void SetValue(float to)
    {
        slider.value = to;
    }
}
