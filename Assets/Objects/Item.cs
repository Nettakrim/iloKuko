using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Item : MonoBehaviour
{
    [SerializeField] private Nimi nimi;

    private SpriteRenderer spriteRenderer;
    private Collider2D bounds;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        bounds = GetComponent<Collider2D>();
    }

    public SpriteRenderer GetSpriteRenderer()
    {
        return spriteRenderer;
    }

    public Bounds GetBounds()
    {
        return bounds.bounds;
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
