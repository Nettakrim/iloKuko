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
        Vector2 mousePos = Global.GetMousePos();
        RectTransform rectTransform = transform as RectTransform;
        if (mousePos.x >= rectTransform.position.x && mousePos.y >= rectTransform.position.y && mousePos.x <= rectTransform.position.x + rectTransform.sizeDelta.x && mousePos.y <= rectTransform.position.y + rectTransform.sizeDelta.y)
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
