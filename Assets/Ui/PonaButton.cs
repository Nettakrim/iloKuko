using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class PonaButton : MonoBehaviour
{
    [SerializeField] private bool hideWebgl;

    public UnityEvent onPress;

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
        if (IsMouseOver())
        {
            UpdateButton(Input.GetKey(KeyCode.Mouse0) ? 2 : 1);
            if (Input.GetKeyUp(KeyCode.Mouse0))
            {
                onPress.Invoke();
            }
        }
        else
        {
            UpdateButton(0);
        }
    }

    protected virtual bool IsMouseOver()
    {
        return Global.MouseOver(transform as RectTransform, true);
    }

    protected abstract void UpdateButton(int state);
}
