using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Global : MonoBehaviour
{
    public static Global instance;
    public Charmap charmap;

    private readonly Dictionary<string, string> map = new();

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

    public string ConvertToSitelenPona(string lasina)
    {
        string s = "";
        foreach (string word in lasina.Split(' '))
        {
            if (map.ContainsKey(word))
            {
                s += map[word];
            }
            else
            {
                Debug.LogWarning("Missing conversion key " + word);
            }
        }
        return s;
    }

    public static float ExpDecay(float a, float b, float decay)
    {
        return b + (a - b) * Mathf.Exp(-decay * Time.deltaTime);
    }
}
