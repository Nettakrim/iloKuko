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

    void Start()
    {
        previousHolds = new List<Object>(maxOrder);
    }

    void Update()
    {
        Vector3 pos = cam.ScreenToWorldPoint(Input.mousePosition);

        if (held)
        {
            held.transform.position = dragOffset + pos;
        } else if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            RaycastHit2D hit2D = Physics2D.Raycast(pos, Vector3.forward, 15);
            if (hit2D)
            {
                held = hit2D.collider.GetComponent<Object>();
                dragOffset = hit2D.transform.position - pos;

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
            }
        }

        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            held = null;
        }
    }
}
