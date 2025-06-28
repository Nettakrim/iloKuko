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
        string replace = "?!.";
        foreach (char c in replace)
        {
            lasina = lasina.Replace(c + " ", " \n ").Replace(c + "", "");
        }

        string s = "";
        foreach (string word in lasina.Split(' '))
        {
            if (map.ContainsKey(word))
            {
                s += map[word];
            }
            else if (word == "\n")
            {
                s += "\n";
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
        return s;
    }

    public static float ExpDecay(float a, float b, float decay)
    {
        return b + (a - b) * Mathf.Exp(-decay * Time.deltaTime);
    }

    public static Vector2 GetMousePos()
    {
        return mousePos;
    }
}
