using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class PonaButton : MonoBehaviour
{
    [SerializeField] private bool hideWebgl;

    public UnityEvent onPress;

    private bool pressed;

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

        if (IsMouseOver())
        {
            UpdateButton((pressed && Input.GetKey(KeyCode.Mouse0)) ? 2 : 1);
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                pressed = true;
            }
            if (up && pressed)
            {
                onPress.Invoke();
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
    }

    protected virtual bool IsMouseOver()
    {
        return Global.MouseOver(transform as RectTransform, true);
    }

    protected abstract void UpdateButton(int state);
}
