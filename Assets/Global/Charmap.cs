using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Charmap : ScriptableObject
{
    public List<Dict> map;

    [System.Serializable]
    public class Dict
    {
        public string key;
        public string value;
    }

    #if UNITY_EDITOR
    public bool ContainsKey(string key)
    {
        foreach (Dict dict in map)
        {
            if (dict.key == key)
            {
                return true;
            }
        }
        return false;
    }
    #endif
}