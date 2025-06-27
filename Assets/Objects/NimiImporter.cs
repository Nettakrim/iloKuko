#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.AssetImporters;
using UnityEngine;

[ScriptedImporter(1, "nimi")]
public class NimiImporter : ScriptedImporter
{
    public override void OnImportAsset(AssetImportContext ctx)
    {

        int lastFolder = ctx.assetPath.LastIndexOf('/');
        string[] assets = AssetDatabase.FindAssets(ctx.assetPath[(lastFolder + 1)..^5] + " t:Sprite a:assets", new[] { ctx.assetPath[..lastFolder] });

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

        List<string> suli = new List<string>();
        List<string> lili = new List<string>();

        bool start = true;

        StreamReader reader = new StreamReader(ctx.assetPath);
        while (!reader.EndOfStream)
        {
            string line = reader.ReadLine();
            (start ? suli : lili).AddRange(line.Split(',', StringSplitOptions.RemoveEmptyEntries));
            start = false;
        }

        Nimi nimi = ScriptableObject.CreateInstance<Nimi>();
        nimi.sprite = sprite;
        nimi.suli = suli.ToArray();
        nimi.lili = lili.ToArray();

        suli.AddRange(lili);
        Charmap charmap = (Charmap)AssetDatabase.LoadAssetAtPath("Assets/Global/ucsur.charmap", typeof(Charmap));
        foreach (string s in suli)
        {
            if (!charmap.ContainsKey(s))
            {
                Debug.LogWarning(".nimi " + ctx.assetPath + " uses unknown nimi " + s);
            }
        }

        ctx.AddObjectToAsset("MainAsset", nimi);
        ctx.SetMainObject(nimi);
    }
}
#endif