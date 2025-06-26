using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Item : MonoBehaviour
{
    [SerializeField] private Nimi nimi;

    private SpriteRenderer spriteRenderer;
    private Collider2D bounds;

    #if UNITY_EDITOR
    [SerializeField] private bool updateSprite;
    #endif

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        bounds = GetComponent<Collider2D>();

        #if UNITY_EDITOR
        if (nimi.sprite.pixelsPerUnit == 100)
        {
            Debug.LogWarning("Sprite " + nimi.sprite + "hasnt been set up");
        }
        #endif
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
        if (nimi == null)
        {
            return;
        }

        if (updateSprite || GetComponent<SpriteRenderer>().sprite != nimi.sprite)
        {
            EditorApplication.delayCall += UpdateSprite;
        }
        updateSprite = false;
    }

    public void SetNimi(Nimi nimi)
    {
        this.nimi = nimi;
        UpdateSprite();
    }

    private void UpdateSprite()
    {
        transform.name = nimi.name;
        GetComponent<SpriteRenderer>().sprite = nimi.sprite;

        DestroyImmediate(GetComponent<PolygonCollider2D>());
        PolygonCollider2D collider2D = gameObject.AddComponent<PolygonCollider2D>();
        collider2D.pathCount = 1;

        EditorUtility.SetDirty(gameObject);

        if (nimi.sprite.pixelsPerUnit == 100)
        {
            Debug.LogWarning("Sprite " + nimi.sprite + "hasnt been set up");
        }
    }
    #endif
}
