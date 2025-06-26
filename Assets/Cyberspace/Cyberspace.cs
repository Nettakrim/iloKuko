using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cyberspace : MonoBehaviour
{
    [SerializeField] private Info[] infos;
    [SerializeField] private Graphic[] graphics;

    [SerializeField] private Transform box;
    [SerializeField] private Transform floor;
    [SerializeField] private Transform top;
    
    private float current;
    private int target;
    [SerializeField] private float speed = 25;

    [SerializeField] private float angleStep;

    private void Start()
    {
        target = 15;
        current = target;
        SetImages(target);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            target--;
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            target++;
        }

        float prev = current;
        current = Global.ExpDecay(current, target, speed);

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

        box.rotation = Quaternion.Euler(0, Mathf.Round((current % 1) * -90/angleStep)*angleStep, 0);
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



    [System.Serializable]
    private class Info
    {
        public Sprite sprite;
        public Color color;
    }

    [System.Serializable]
    private class Graphic
    {
        public SpriteRenderer a;
        public SpriteRenderer b;
        public SpriteRenderer c;

        public void Set(Info info)
        {
            a.sprite = info.sprite;
            b.sprite = info.sprite;
            c.sprite = info.sprite;
            a.color = info.color;
            b.color = info.color;
            c.color = info.color;
        }
    }
}
