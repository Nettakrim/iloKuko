using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Object : MonoBehaviour
{
    [SerializeField] private Nimi nimi;

    private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public SpriteRenderer GetSpriteRenderer()
    {
        return spriteRenderer;
    }

    #if UNITY_EDITOR
    void OnValidate()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer.sprite != nimi.sprite)
        {
            EditorApplication.delayCall += () =>
            {
                transform.name = nimi.name;
                spriteRenderer.sprite = nimi.sprite;

                DestroyImmediate(GetComponent<PolygonCollider2D>());
                PolygonCollider2D collider2D = gameObject.AddComponent<PolygonCollider2D>();
                collider2D.pathCount = 1;

                EditorUtility.SetDirty(gameObject);
            };
        }
    }
    #endif
}
