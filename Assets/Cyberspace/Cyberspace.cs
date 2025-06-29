using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Cyberspace : MonoBehaviour
{
    [SerializeField] private Info[] infos;
    [SerializeField] private Graphic[] graphics;

    [SerializeField] private Transform doors;
    [SerializeField] private Transform floor;
    [SerializeField] private Transform top;

    private float current;
    private int target;
    [SerializeField] private float speed = 25;

    [SerializeField] private float angleStep;

    [SerializeField] private RawImage screen;
    private Material material;
    [SerializeField] private AnimationCurve transitionCurve;
    [SerializeField] private float transitionDuration;

    private bool inCyberspace = true;
    private State state = State.Cyberspace;
    private float transitionedAt;

    [SerializeField] private Camera cam;

    [SerializeField] private BoxCollider2D boxBounds;
    [SerializeField] private float tiltThreshold;
    [SerializeField] private float untiltThreshold;
    [SerializeField] private float tilt;
    private bool tilted;

    private int mouseAction = -1;

    [SerializeField] private SitelenPonaImage button;
    private bool lockInput;

    private void Start()
    {
        target = 17;
        current = target;
        SetImages(target);

        material = screen.material;
        SetTransitionAnimation(0);

        foreach (Info info in infos)
        {
            info.SetActive(false, null);
        }
        button.gameObject.SetActive(false);

        UpdateRotation();
    }

    private void Update()
    {
        UpdateHover();

        if (lockInput || Global.ignoreMask > 0)
        {
            return;
        }

        UpdateRotation();
        UpdateTransition();
    }

    private void UpdateHover()
    {
        if (state != State.Cyberspace && state != State.Exiting)
        {
            return;
        }

        Vector2 mousePos = Global.GetMousePos();
        if (!boxBounds.OverlapPoint(mousePos))
        {
            return;
        }

        float viewPort = (mousePos.x - boxBounds.offset.x) / boxBounds.size.x;

        if (!tilted && Mathf.Abs(viewPort) > tiltThreshold)
        {
            tilted = true;
        }
        else if (tilted && Mathf.Abs(viewPort) < untiltThreshold)
        {
            tilted = false;
        }

        if (tilted)
        {
            cam.transform.localRotation = Quaternion.Euler(0, viewPort > 0 ? tilt : -tilt, 0);
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                mouseAction = viewPort > 0 ? 0 : 1;
            }
            if (Input.GetKeyUp(KeyCode.Mouse0) && mouseAction == (viewPort > 0 ? 0 : 1))
            {
                Cycle(viewPort > 0 ? 1 : -1);
            }
        }
        else
        {
            cam.transform.localRotation = Quaternion.identity;
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                mouseAction = 2;
            }
            if (Input.GetKeyUp(KeyCode.Mouse0) && mouseAction == 2)
            {
                SetCyberspace(!inCyberspace);
            }
        }

        if (Input.GetKeyUp(KeyCode.Mouse0) || lockInput || Global.ignoreMask > 0)
        {
            mouseAction = -1;
        }
    }

    private void UpdateRotation()
    {
        float prev = current;
        current = Mathf.MoveTowards(current, target, speed*Time.deltaTime);

        int c = Mathf.FloorToInt(current);
        if (c != Mathf.FloorToInt(prev))
        {
            if (c > 30)
            {
                current -= 20;
                target -= 20;
            }
            if (c < 10)
            {
                current += 20;
                target += 20;
            }

            SetImages(c);

        }

        float angle = Mathf.Round((current % 1) * -90 / angleStep) * angleStep;
        doors.rotation = Quaternion.Euler(0, angle, 0);
        float r = Mathf.Floor(current) * -90;
        floor.localRotation = Quaternion.Euler(-90, r, 0);
        top.localRotation = Quaternion.Euler(90, r, 0);
    }

    private void SetImages(int index)
    {
        for (int i = 0; i < 4; i++)
        {
            graphics[i].Set(infos[(i + index) % 5]);
        }
    }

    private void UpdateTransition()
    {
        if (state == State.Cyberspace && !inCyberspace)
        {
            state = State.Entering;
            transitionedAt = Time.time;
            infos[(target + 1) % 5].SetActive(true, button);
            button.gameObject.SetActive(true);
            cam.enabled = false;
        }
        else if (state == State.Box && inCyberspace)
        {
            state = State.Exiting;
            transitionedAt = Time.time;
            cam.enabled = true;
        }

        if (state == State.Entering)
        {
            float t = GetTransitionTime();
            SetTransitionAnimation(transitionCurve.Evaluate(t));
            if (t >= 1)
            {
                state = State.Box;
            }
        }
        if (state == State.Exiting)
        {
            float t = GetTransitionTime();
            SetTransitionAnimation(-transitionCurve.Evaluate(1 - t));
            if (t >= 1)
            {
                state = State.Cyberspace;
                infos[(target + 1) % 5].SetActive(false, null);
                button.gameObject.SetActive(false);
            }
        }
    }

    public void Cycle(int direction)
    {
        if (state == State.Cyberspace && inCyberspace)
        {
            target += direction;
        }
    }

    public void SetCyberspace(bool to)
    {
        inCyberspace = to;
    }

    public float GetTransitionTime()
    {
        return (Time.time - transitionedAt) / transitionDuration;
    }

    public void SetTransitionAnimation(float t)
    {
        material.SetFloat("_NoiseAmplitude", t);
    }

    public enum State
    {
        Cyberspace,
        Entering,
        Box,
        Exiting
    }

    public int GetCurrentBox()
    {
        return target % 5;
    }

    public State GetState()
    {
        return state;
    }

    public void LockInput(bool locked)
    {
        lockInput = locked;
    }

    [Serializable]
    private class Info
    {
        [SerializeField] private Sprite sprite;
        [SerializeField] private Color color;
        [SerializeField] private GameObject box;
        [SerializeField] private Sprite buttonLasina;
        [SerializeField] private Sprite buttonPona;

        public void SetActive(bool active, SitelenPonaImage image)
        {
            box.SetActive(active);
            if (active)
            {
                image.Set(buttonLasina, buttonPona);
            }
        }

        public Sprite GetSprite()
        {
            return sprite;
        }

        public Color GetColor()
        {
            return color;
        }
    }

    [Serializable]
    private class Graphic
    {
        [SerializeField] private SpriteRenderer a;
        [SerializeField] private SpriteRenderer b;
        [SerializeField] private SpriteRenderer c;

        public void Set(Info info)
        {
            Sprite sprite = info.GetSprite();
            Color color = info.GetColor();
            a.sprite = sprite;
            b.sprite = sprite;
            c.sprite = sprite;
            a.color = color;
            b.color = color;
            c.color = color;
        }
    }

    #if UNITY_EDITOR
    void OnDestroy()
    {
        SetTransitionAnimation(1);
    }
    #endif
}
