using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextManager : MonoBehaviour
{
    public static bool useSitelenPona { get; private set; }

    [SerializeField] private RectTransform messageParent;
    [SerializeField] private SitelenPona messagePrefab;

    private float scroll = -1f;
    private bool scrollEnabled = false;
    [SerializeField] private float scrollSpeed;

    private float height;

    private static bool refreshLayout;

    private static TextManager instance;

    [SerializeField] private Creature[] creatures;

    [Serializable]
    private class Creature
    {
        public string name;
        public Sprite sprite;
    }

    void Start()
    {
        instance = this;
        height = (messageParent.parent as RectTransform).sizeDelta.y;
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
            LayoutRebuilder.ForceRebuildLayoutImmediate(messageParent);
        }

        float over = messageParent.sizeDelta.y - height;

        if (scrollEnabled)
        {
            if (over < 0f)
            {
                SetLayoutAnchor(1f);
                scrollEnabled = false;
                messageParent.anchoredPosition = new Vector2(0, 0);
                scroll = -1f;
                return;
            }

            float y = Input.mouseScrollDelta.y;
            if ((y == 0 && scroll <= over) || !Global.MouseOver(transform as RectTransform, true))
            {
                return;
            }

            float delta = y * scrollSpeed;

            if (scroll == -1)
            {
                if (delta > 0)
                {
                    SetLayoutAnchor(1f);
                    scroll = Mathf.Floor(over / scrollSpeed) * scrollSpeed;
                }
                else
                {
                    return;
                }
            }

            scroll = Mathf.Clamp(scroll - delta, 0f, over);
            messageParent.anchoredPosition = new Vector2(0, scroll);

            if (scroll > over - scrollSpeed)
            {
                scroll = -1;
            }

            if (scroll == -1)
            {
                messageParent.anchoredPosition = new Vector2(0, 0);
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
        messageParent.pivot = new Vector2(0.5f, to);
        messageParent.anchorMin = new Vector2(0f, to);
        messageParent.anchorMax = new Vector2(1f, to);
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

    public static void CreateMessage(string message)
    {
        int i = message.IndexOf(']');
        string name = message[1..i];
        Sprite sprite = null;

        foreach (Creature creature in instance.creatures)
        {
            if (creature.name == name)
            {
                sprite = creature.sprite;
                break;
            }
        }

        SitelenPona sitelenPona = Instantiate(instance.messagePrefab, instance.messageParent);
        sitelenPona.SetMessage(message[(i + 1)..]);
        sitelenPona.transform.GetChild(0).GetComponent<Image>().sprite = sprite;
        UpdateLayout();

        if (sprite == null)
        {
            Debug.Log("Couldnt find character " + name);
        }
    }
}
