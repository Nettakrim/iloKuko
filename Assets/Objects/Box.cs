using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Box : MonoBehaviour
{
    [SerializeField] private Camera cam;

    private Item held;
    private Vector3 dragOffset;

    private List<Item> previousHolds;

    [SerializeField] private Vector3 heldScale;
    [SerializeField] private float jostleAmount;

    [SerializeField] private float forgivenessRadius;
    [SerializeField] private LayerMask layerMask;

    [SerializeField] private BoxCollider2D boxBounds;

    #if UNITY_EDITOR
    [SerializeField] private Material imageMaterial;
    [SerializeField] private bool generateMissingItems;
    #endif

    [SerializeField] private float rotateSpeed;

    private void Start()
    {
        previousHolds = new List<Item>(transform.childCount);
        foreach (Transform child in transform)
        {
            previousHolds.Add(child.GetComponent<Item>());
        }
        previousHolds.Reverse();
        UpdateObjectOrder();
    }

    private void Update()
    {
        Vector3 mousePos = Global.GetMousePos();
        mousePos = new Vector3(Mathf.Round(mousePos.x), Mathf.Round(mousePos.y), 0);

        if (held)
        {
            float scroll = Input.mouseScrollDelta.y;
            if (scroll != 0)
            {
                Quaternion rotation = Quaternion.Euler(0, 0, scroll * rotateSpeed);
                held.transform.localRotation *= rotation;
                dragOffset = rotation * dragOffset;
            }

            held.transform.localPosition = dragOffset + mousePos;

            if (Input.GetKeyUp(KeyCode.Mouse0))
            {
                Jostle(mousePos, Vector3.one);

                Vector4 bounds = GetBounds(held.GetBounds());
                held.transform.localPosition = new Vector3(Mathf.Clamp(held.transform.localPosition.x, bounds.x, bounds.z), Mathf.Clamp(held.transform.localPosition.y, bounds.y, bounds.w), 0);

                held = null;
            }
        }
        else if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            held = RaycastItems(mousePos, 0);

            if (!held)
            {
                held = RaycastItems(mousePos, forgivenessRadius);
            }

            if (held)
            {
                dragOffset = held.transform.localPosition - mousePos;

                previousHolds.Remove(held);
                previousHolds.Add(held);

                UpdateObjectOrder();

                Jostle(mousePos, heldScale);
            }
        }
    }

    private Item RaycastItems(Vector3 pos, float radius)
    {
        Item top = null;
        RaycastHit2D[] hits = radius == 0 ? Physics2D.RaycastAll(pos, Vector2.up, 0.1f, layerMask) : Physics2D.CircleCastAll(pos, radius, Vector2.up, 0.1f, layerMask);
        foreach (RaycastHit2D hit in hits)
        {
            Item hitItem = hit.transform.GetComponent<Item>();

            if (hitItem && (!top || hitItem.GetSpriteRenderer().sortingOrder > top.GetSpriteRenderer().sortingOrder))
            {
                top = hitItem;
            }
        }

        return top;
    }

    private void UpdateObjectOrder()
    {
        for (int i = 0; i < previousHolds.Count; i++)
        {
            previousHolds[i].GetSpriteRenderer().sortingOrder = i + 1;
        }
    }

    private void Jostle(Vector3 mousePos, Vector3 scale)
    {
        Quaternion rotation = Quaternion.Euler(0, 0, Random.Range(-jostleAmount, jostleAmount));
        dragOffset = rotation * new Vector3(dragOffset.x * scale.x / held.transform.localScale.x, dragOffset.y * scale.y / held.transform.localScale.y, dragOffset.z);

        held.transform.localScale = scale;
        held.transform.localRotation *= rotation;
        held.transform.localPosition = dragOffset + mousePos;
    }

    private void OnDisable()
    {
        held = null;
    }

    private Vector4 GetBounds(Bounds bounds) {
        Vector2 min = boxBounds.offset - boxBounds.size / 2 + (Vector2)bounds.extents / 2;
        Vector2 max = boxBounds.offset + boxBounds.size / 2 - (Vector2)bounds.extents / 2;
        return new Vector4(min.x, min.y, max.x, max.y);
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (!generateMissingItems)
        {
            return;
        }

        generateMissingItems = false;

        EditorApplication.delayCall += () =>
        {
            string folder = "Assets/Objects/" + name.ToLower();
            string[] assets = AssetDatabase.FindAssets("t:Nimi a:assets", new[] { folder });

            foreach (string s in assets)
            {
                string path = AssetDatabase.GUIDToAssetPath(s);
                Nimi nimi = (Nimi)AssetDatabase.LoadAssetAtPath(path, typeof(Nimi));
                if (HasImage(nimi.name))
                {
                    continue;
                }

                GameObject obj = new GameObject(nimi.name);
                obj.transform.SetParent(transform);
                obj.layer = gameObject.layer;
                obj.AddComponent<SpriteRenderer>().sharedMaterial = imageMaterial;

                Item item = obj.AddComponent<Item>();
                item.SetNimi(nimi);

                Vector4 bounds = GetBounds(item.GetComponent<Collider2D>().bounds);
                obj.transform.SetLocalPositionAndRotation(new Vector3(Random.Range(bounds.x, bounds.z), Random.Range(bounds.y, bounds.w), 0), Quaternion.Euler(0, 0, Random.Range(0, 360)));
            }
        };

        EditorUtility.SetDirty(gameObject);
    }

    private bool HasImage(string name)
    {
        foreach (Transform child in transform)
        {
            if (child.name == name)
            {
                return true;
            }
        }
        return false;
    }
    #endif
}
