using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextManager : MonoBehaviour
{
    public static bool useSitelenPona { get; private set; }

    [SerializeField] private RectTransform layout;

    private float scroll = -1f;
    private bool scrollEnabled = false;
    [SerializeField] private float scrollSpeed;

    private float height;

    private static bool refreshLayout;

    void Start()
    {
        height = (layout.parent as RectTransform).sizeDelta.y;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            ToggleSitelenPona();
        }
    }

    void LateUpdate()
    {
        if (refreshLayout)
        {
            refreshLayout = false;
            LayoutRebuilder.ForceRebuildLayoutImmediate(layout);
        }

        float over = layout.sizeDelta.y - height;

        if (scrollEnabled)
        {
            if (over < 0f)
            {
                SetLayoutAnchor(1f);
                scrollEnabled = false;
                layout.anchoredPosition = new Vector2(0, 0);
                scroll = -1f;
                return;
            }

            float y = Input.mouseScrollDelta.y;
            if (y == 0 && scroll <= over)
            {
                return;
            }

            float delta = y * scrollSpeed;

            if (scroll == -1)
            {
                if (delta > 0)
                {
                    SetLayoutAnchor(1f);
                    scroll = Mathf.Floor(over/scrollSpeed)*scrollSpeed;
                }
                else
                {
                    return;
                }
            }

            scroll = Mathf.Clamp(scroll - delta, 0f, over);
            layout.anchoredPosition = new Vector2(0, scroll);

            if (scroll > over - scrollSpeed)
            {
                scroll = -1;
            }

            if (scroll == -1)
            {
                layout.anchoredPosition = new Vector2(0, 0);
                SetLayoutAnchor(0f);
            }
        }
        else if (over > 0f)
        {
            SetLayoutAnchor(0f);
            scrollEnabled = true;
        }
    }

    void SetLayoutAnchor(float to)
    {
        layout.pivot = new Vector2(0.5f, to);
        layout.anchorMin = new Vector2(0f, to);
        layout.anchorMax = new Vector2(1f, to);
    }

    public void ToggleSitelenPona()
    {
        useSitelenPona = !useSitelenPona;
        UpdateLayout();
    }

    public static void UpdateLayout()
    {
        refreshLayout = true;
    }
}
