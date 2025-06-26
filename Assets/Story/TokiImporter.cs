#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor.AssetImporters;
using UnityEngine;

[ScriptedImporter(1, "toki")]
public class TokiImporter : ScriptedImporter
{
    public override void OnImportAsset(AssetImportContext ctx)
    {
        List<Expression.Wrapper> expressions = new();

        Reader reader = new Reader(new StreamReader(ctx.assetPath));
        while (reader.Next())
        {
            Expression expression = Expression.ParseReader(reader);
            if (expression == null)
            {
                Debug.LogWarning("Couldnt pass expression for .toki \"" + ctx.assetPath);
            }
            else
            {
                expressions.Add(expression);
            }
        }

        Toki toki = ScriptableObject.CreateInstance<Toki>();
        toki.expressions = expressions.ToArray();

        ctx.AddObjectToAsset("MainAsset", toki);
        ctx.SetMainObject(toki);
    }
}
#endif