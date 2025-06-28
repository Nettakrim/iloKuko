using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class FakeButton : MonoBehaviour
{
    [SerializeField] private bool hideWebgl;

    private Image image;

    [SerializeField] private Color hover;
    [SerializeField] private Color press;

    public UnityEvent onPress;

    void Start()
    {
        image = GetComponent<Image>();

        #if UNITY_WEBGL
        if (hideWebgl) {
            Destroy(gameObject);
        }
        #endif
    }


    void Update()
    {
        if (Global.MouseOver(transform as RectTransform))
        {
            image.color = Input.GetKey(KeyCode.Mouse0) ? press : hover;
            if (Input.GetKeyUp(KeyCode.Mouse0))
            {
                onPress.Invoke();
            }
        }
        else
        {
            image.color = Color.white;
        }
    }
}
