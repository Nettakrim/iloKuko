#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor.AssetImporters;
using UnityEngine;

[ScriptedImporter(1, "charmap")]
public class CharmapImporter : ScriptedImporter
{
    public override void OnImportAsset(AssetImportContext ctx)
    {
        Charmap charmap = ScriptableObject.CreateInstance<Charmap>();
        charmap.map = new();

        StreamReader reader = new StreamReader(ctx.assetPath);
        while (!reader.EndOfStream)
        {
            string line = reader.ReadLine();
            charmap.map.Add(new() {key = line[..^2], value = line[^2..]});
        }

        ctx.AddObjectToAsset("MainAsset", charmap);
        ctx.SetMainObject(charmap);
    }
}
#endif