using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class TextManager : MonoBehaviour
{
    private static TextManager instance;

    private static bool useSitelenPona;
    private static readonly UnityEvent<bool> onSitelenChange = new UnityEvent<bool>();

    [SerializeField] private RectTransform messageParent;
    [SerializeField] private SitelenPona messagePrefab;

    private float scroll = -1f;
    private bool scrollEnabled = false;
    [SerializeField] private float scrollSpeed;

    private float height;

    private static bool refreshLayout;

    [SerializeField] private Creature[] creatures;

    private float animationTime;
    [SerializeField] private float animationSpeed;
    [SerializeField] private float animationSteps;
    [SerializeField] private float speakDuration;

    [SerializeField] private Sprite kuko;
    [SerializeField] private SoundGroup kukoSearch;
    [SerializeField] private Color kukoColor;

    [SerializeField] private Image background;
    [SerializeField] private Sprite[] backgroundSprites;

    [SerializeField] private GameObject[] pini;

    [SerializeField] private AudioClip[] music;

    [SerializeField] private GameObject[] buttons;

    [Serializable]
    private class Creature
    {
        public string name;
        public Sprite sprite;
        public Color color;

        [SerializeField] private RectTransform transform;
        [SerializeField] private float[] positions;
        [SerializeField] private float silent;
        [SerializeField] private float speaking;

        private float previousPosition;
        private int targetPosition;
        private float speakTime;

        [SerializeField] private SoundGroup speak;

        public void Update(float time)
        {
            transform.anchoredPosition = new Vector2(Mathf.Lerp(positions[targetPosition], previousPosition, time), speakTime > 0 ? speaking : silent);
            if (time == 0)
            {
                previousPosition = transform.anchoredPosition.x;
            }
            
            if (speakTime > 0)
            {
                speakTime -= Time.deltaTime;
            }
        }

        public void Speak(float duration)
        {
            speakTime = duration;
            speak.Play(true);
        }

        public void ChangeTarget(int to)
        {
            targetPosition = to;
            previousPosition = transform.anchoredPosition.x;
        }
    }

    void Start()
    {
        instance = this;
        height = (messageParent.parent as RectTransform).sizeDelta.y;

        if (!useSitelenPona && PlayerPrefs.GetInt("useSitelenPona") > 0)
        {
            ToggleSitelenPona();
        }

        for (int i = 0; i < creatures.Length; i++)
        {
            creatures[i].ChangeTarget(0);
        }
    }

    void Update()
    {
        float t;
        if (animationTime > 0)
        {
            animationTime -= Time.deltaTime * animationSpeed;
            if (animationTime < 0)
            {
                animationTime = 0;
            }
            t = Mathf.Floor(animationTime * animationSteps) / animationSteps;
        }
        else
        {
            t = 0;
        }

        for (int i = 0; i < creatures.Length; i++)
        {
            creatures[i].Update(t);
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
            if ((y == 0 && scroll <= over) || !Global.MouseOver(transform as RectTransform, 3))
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
        onSitelenChange.Invoke(useSitelenPona);
        UpdateLayout();

        scroll = -1;
        if (messageParent.sizeDelta.y - height < 0)
        {
            SetLayoutAnchor(1f);
            messageParent.anchoredPosition = new Vector2(0, 0);
            scrollEnabled = false;
        }
        else
        {
            SetLayoutAnchor(0f);
            messageParent.anchoredPosition = new Vector2(0, 0);
            scrollEnabled = true;
        }

        for (int i = 0; i < buttons.Length; i++)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(buttons[i].transform as RectTransform);
        }

        PlayerPrefs.SetInt("useSitelenPona", useSitelenPona ? 1 : 0);
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
        Color color = Color.black;

        foreach (Creature creature in instance.creatures)
        {
            if (creature.name == name)
            {
                sprite = creature.sprite;
                creature.Speak(instance.speakDuration);
                color = creature.color;
                break;
            }
        }

        if (name.StartsWith("kuko"))
        {
            sprite = instance.kuko;
            instance.kukoSearch.Play(true);
            color = instance.kukoColor;
        }

        SitelenPona sitelenPona = Instantiate(instance.messagePrefab, instance.messageParent);
        sitelenPona.SetMessage(message[(i + 2)..], " ");
        sitelenPona.GetText().color = color;

        Image image = sitelenPona.transform.GetChild(0).GetComponent<Image>();
        image.sprite = sprite;
        if (!name.StartsWith("kuko"))
        {
            image.color = color;
        }
        UpdateLayout();

        if (sprite == null)
        {
            Debug.Log("Couldnt find character " + name);
        }
    }

    public static void Stage(string direction)
    {
        string[] parts = direction.Split(" -> ");
        int target = int.Parse(parts[1], CultureInfo.InvariantCulture);

        if (parts[0] == "background")
        {
            instance.background.sprite = instance.backgroundSprites[target];
            if (target == 2)
            {
                instance.pini[0].SetActive(false);
                instance.pini[1].SetActive(true);
            }
            return;
        }

        if (parts[0] == "music")
        {
            MusicManager.instance.PlayMusic(instance.music[target]);
            return;
        }

        foreach (Creature creature in instance.creatures)
        {
            if (creature.name == parts[0])
            {
                creature.ChangeTarget(target);
            }
        }
        instance.animationTime = 1f;
    }

    public static void RegisterSitelenChange(UnityAction<bool> action)
    {
        onSitelenChange.AddListener(action);
        action.Invoke(useSitelenPona);
    }
}
