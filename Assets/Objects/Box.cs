using System.Collections;
using System.Collections.Generic;
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
        Vector2 mouse = Input.mousePosition;
        mouse /= new Vector2(Screen.width, Screen.height);
        mouse -= new Vector2(0.5f, 0.5f);
        mouse *= new Vector2(384, 216);
        
        float aspect = (Screen.width / (float)Screen.height) / (16f / 9f);
        if (aspect > 1)
        {
            mouse.x *= aspect;
        }
        else if (aspect < 1)
        {
            mouse.y /= aspect;
        }

        Vector3 mousePos = mouse;

        if (held)
        {
            held.transform.localPosition = dragOffset + mousePos;

            if (Input.GetKeyUp(KeyCode.Mouse0))
            {
                Jostle(mousePos, Vector3.one);

                Bounds heldBounds = held.GetBounds();
                Vector2 max = boxBounds.offset + boxBounds.size/2 - (Vector2)heldBounds.extents/2;
                Vector2 min = boxBounds.offset - boxBounds.size/2 + (Vector2)heldBounds.extents/2;
                held.transform.localPosition = new Vector3(Mathf.Clamp(held.transform.localPosition.x, min.x, max.x), Mathf.Clamp(held.transform.localPosition.y, min.y, max.y), 0);

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
}
