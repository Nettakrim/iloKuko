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
        charmap.map = new Dictionary<string, char>();

        StreamReader reader = new StreamReader(ctx.assetPath);
        while (!reader.EndOfStream)
        {
            string line = reader.ReadLine();
            charmap.map.Add(line[..^1], line[^1]);
        }

        ctx.AddObjectToAsset("MainAsset", charmap);
        ctx.SetMainObject(charmap);
    }
}
