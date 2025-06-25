using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Charmap : ScriptableObject
{
    public List<Dict> map;

    [System.Serializable]
    public struct Dict
    {
        public string key;
        public string value;
    }
}