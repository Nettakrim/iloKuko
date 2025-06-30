using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Global : MonoBehaviour
{
    public static Global instance;
    public Charmap charmap;

    private readonly Dictionary<string, string> map = new();

    private static Vector2 mousePos;

    public static int ignoreMask;

    void Awake()
    {
        if (instance)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;

        foreach (Charmap.Dict dict in charmap.map)
        {
            map.Add(dict.key, dict.value);
        }
    }

    void Update()
    {
        mousePos = Input.mousePosition;
        mousePos /= new Vector2(Screen.width, Screen.height);
        mousePos -= new Vector2(0.5f, 0.5f);
        mousePos *= new Vector2(384, 216);

        float aspect = (Screen.width / (float)Screen.height) / (16f / 9f);
        if (aspect > 1)
        {
            mousePos.x *= aspect;
        }
        else if (aspect < 1)
        {
            mousePos.y /= aspect;
        }
    }

    public string ConvertToSitelenPona(string lasina)
    {
        if (lasina.StartsWith("a!"))
        {
            lasina = "a" + lasina[2..];
        }

        string replace = "?!.";
        foreach (char c in replace)
        {
            lasina = lasina.Replace(c + " ", " \n ").Replace(c + "", "");
        }

        lasina = lasina.Replace(", li ", " li ").Replace(", e ", " e ").Replace(", ", " 󱦜 ").Replace("~","");

        string s = "";
        foreach (string word in lasina.Split(' '))
        {
            if (map.ContainsKey(word))
            {
                s += map[word];
            }
            else if (word == "\n" || word == ">" || word == "󱦜")
            {
                s += word;
            }
            else
            {
                s += "󱦐";
                foreach (char c in word)
                {
                    s += c + "󱦒";
                }
                s += "󱦑";
            }
        }
        return s.Replace(">",">  ");
    }

    public static float ExpDecay(float a, float b, float decay)
    {
        return b + (a - b) * Mathf.Exp(-decay * Time.deltaTime);
    }

    public static Vector2 GetMousePos()
    {
        return mousePos;
    }

    public static bool MouseOver(RectTransform rectTransform, int mask)
    {
        if ((ignoreMask & mask) > 0)
        {
            return false;
        }

        Vector2 pos = rectTransform.position;
        Vector2 size = rectTransform.sizeDelta;
        Vector2 pivot = rectTransform.pivot;
        return mousePos.x >= pos.x - size.x*pivot.x && mousePos.y >= pos.y - size.y*pivot.y && mousePos.x <= pos.x + size.x*(1-pivot.x) && mousePos.y <= pos.y + size.y*(1-pivot.y);
    }
}
