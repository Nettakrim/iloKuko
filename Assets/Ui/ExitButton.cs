using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitButton : FakeButton
{
    [SerializeField] private float pressDown;

    [SerializeField] private float radiusSquared;

    protected override void UpdateButton(int state)
    {
        base.UpdateButton(state);
        (transform as RectTransform).anchoredPosition = state == 2 ? new Vector2(0, -pressDown) : Vector2.zero;
    }

    protected override bool IsMouseOver()
    {
        return (Global.GetMousePos() - ((Vector2)transform.parent.position)).sqrMagnitude < radiusSquared;
    }
}
