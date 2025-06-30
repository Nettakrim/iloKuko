#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.AssetImporters;
using UnityEngine;

[ScriptedImporter(1, "nimi")]
public class NimiImporter : ScriptedImporter
{
    public override void OnImportAsset(AssetImportContext ctx)
    {

        int lastFolder = ctx.assetPath.LastIndexOf('/');
        string name = ctx.assetPath[(lastFolder + 1)..^5];
        string[] assets = AssetDatabase.FindAssets(name + " t:Sprite a:assets", new[] { ctx.assetPath[..lastFolder] });

        Sprite sprite = null;
        foreach (string s in assets)
        {
            string path = AssetDatabase.GUIDToAssetPath(s);
            if (path.StartsWith(ctx.assetPath[..^5]))
            {
                sprite = (Sprite)AssetDatabase.LoadAssetAtPath(path, typeof(Sprite));
                break;
            }
        }

        if (sprite == null)
        {
            Debug.LogWarning("Couldn't find matching sprite for .nimi \"" + ctx.assetPath + "\"");
            return;
        }

        List<Nimi.Layer> words = new();

        StreamReader reader = new StreamReader(ctx.assetPath);
        while (!reader.EndOfStream)
        {
            string line = reader.ReadLine();
            words.Add(new Nimi.Layer(line.Split(',', StringSplitOptions.RemoveEmptyEntries)));
        }
        reader.Close();

        Nimi nimi = ScriptableObject.CreateInstance<Nimi>();
        nimi.sprite = sprite;
        nimi.words = words.ToArray();

        bool hasColor = false;
        string targetColor = ctx.assetPath[..^(6+name.Length)];
        targetColor = targetColor[(targetColor.LastIndexOf('/')+1)..];

        Charmap charmap = (Charmap)AssetDatabase.LoadAssetAtPath("Assets/Global/ucsur.charmap", typeof(Charmap));
        foreach (Nimi.Layer list in words)
        {
            foreach (string word in list)
            {
                if (!charmap.ContainsKey(word))
                {
                    Debug.LogWarning(".nimi " + ctx.assetPath + " uses unknown nimi " + word);
                }

                hasColor |= word == targetColor;
            }
        }

        if (words.Count == 0)
        {
            Debug.LogWarning(".nimi " + ctx.assetPath + " doesnt have any entries");
        } else if (!hasColor)
        {
            Debug.LogWarning(".nimi " + ctx.assetPath + " doesnt have color " + targetColor);
        }

        ctx.AddObjectToAsset("MainAsset", nimi);
        ctx.SetMainObject(nimi);
    }
}
#endif