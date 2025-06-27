using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Nimi : ScriptableObject
{
    public Sprite sprite;

    public Layer[] words;

    [Serializable]
    public class Layer : IEnumerable<string>
    {
        public List<string> words;

        #if UNITY_EDITOR
        public Layer(string[] words)
        {
            this.words = words.ToList();
        }
        #endif

        public IEnumerator<string> GetEnumerator()
        {
            return words.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}