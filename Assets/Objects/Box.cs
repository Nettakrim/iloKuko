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
        Vector3 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);

        if (held)
        {
            held.transform.position = dragOffset + mousePos;

            if (Input.GetKeyUp(KeyCode.Mouse0))
            {
                Jostle(mousePos, Vector3.one);
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
                    dragOffset = held.transform.position - mousePos;

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
        RaycastHit2D[] hits = radius == 0 ? Physics2D.RaycastAll(pos, Vector2.zero, 15) : Physics2D.CircleCastAll(pos, radius, Vector2.zero, 15);
        foreach (RaycastHit2D hit in hits)
        {
            Item hitItem = hit.transform.GetComponent<Item>();

            if (!top || hitItem.GetSpriteRenderer().sortingOrder > top.GetSpriteRenderer().sortingOrder)
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
