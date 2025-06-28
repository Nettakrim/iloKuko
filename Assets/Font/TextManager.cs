using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextManager : MonoBehaviour
{
    public static bool useSitelenPona;

    [SerializeField] private RectTransform layout;

    private float scroll = 0f;
    private bool scrollEnabled = false;
    [SerializeField] private float scrollSpeed;

    private float height;
    private float lastSize;

    void Start()
    {
        height = (layout.parent as RectTransform).sizeDelta.y;
    }

    void LateUpdate()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            useSitelenPona = !useSitelenPona;
        }

        float over = layout.sizeDelta.y - height;

        if (scrollEnabled)
        {
            if (over < 0f)
            {
                SetLayoutAnchor(1f);
                scrollEnabled = false;
                layout.anchoredPosition = new Vector2(0, 0);
                scroll = 0f;
                return;
            }

            if (scroll != 0 && (over != lastSize))
            {
                scroll += over - lastSize;
            }

            scroll = Mathf.Clamp(scroll + Input.mouseScrollDelta.y * scrollSpeed, 0f, over);
            layout.anchoredPosition = new Vector2(0, -scroll);
        }
        else if (over > 0f)
        {
            SetLayoutAnchor(0f);
            scrollEnabled = true;
        }

        lastSize = over;
    }

    void SetLayoutAnchor(float to)
    {
        layout.pivot = new Vector2(0.5f, to);
        layout.anchorMin = new Vector2(0f, to);
        layout.anchorMax = new Vector2(1f, to);
    }
}
