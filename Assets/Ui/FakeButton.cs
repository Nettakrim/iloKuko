using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FakeButton : GameButton
{
    private Graphic graphic;

    [SerializeField] private Color hover;
    [SerializeField] private Color press;

    protected override void UpdateButton(int state)
    {
        if (graphic == null)
        {
            graphic = GetComponent<Graphic>();
        }
        graphic.color = state == 0 ? Color.white : (state == 1 ? hover : press);
    }
}
