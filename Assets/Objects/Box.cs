using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box : MonoBehaviour
{
    [SerializeField] private Camera cam;

    private Object held;
    private Vector3 dragOffset;

    [SerializeField] private int maxOrder;
    private List<Object> previousHolds;

    [SerializeField] private Vector3 heldScale;
    [SerializeField] private float jostleAmount;

    void Start()
    {
        previousHolds = new List<Object>(maxOrder);
    }

    void Update()
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
            RaycastHit2D hit2D = Physics2D.Raycast(mousePos, Vector3.forward, 15);
            if (hit2D)
            {
                held = hit2D.collider.GetComponent<Object>();
                dragOffset = hit2D.transform.position - mousePos;

                previousHolds.Remove(held);

                if (previousHolds.Count >= maxOrder)
                {
                    previousHolds[0].GetSpriteRenderer().sortingOrder = 0;
                    previousHolds.RemoveAt(0);
                }

                previousHolds.Add(held);
                for (int i = 0; i < previousHolds.Count; i++)
                {
                    previousHolds[i].GetSpriteRenderer().sortingOrder = i + 1;
                }

                Jostle(mousePos, heldScale);
            }
        }
    }

    public void Jostle(Vector3 mousePos, Vector3 scale)
    {
        Quaternion rotation = Quaternion.Euler(0, 0, Random.Range(-jostleAmount, jostleAmount));
        dragOffset = rotation * new Vector3(dragOffset.x * scale.x / held.transform.localScale.x, dragOffset.y * scale.y / held.transform.localScale.y, dragOffset.z);

        held.transform.localScale = scale;
        held.transform.localRotation *= rotation;
        held.transform.localPosition = dragOffset + mousePos;
    }

    void OnDisable()
    {
        held = null;
    }
}
