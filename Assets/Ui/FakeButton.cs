using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FakeButton : GameButton
{
    private Graphic graphic;

    [SerializeField] private Color hover;
    [SerializeField] private Color press;

    void Start()
    {
        graphic = GetComponent<Graphic>();
    }

    protected override void UpdateButton(int state)
    {
        graphic.color = state == 0 ? Color.white : (state == 1 ? hover : press);
    }
}
