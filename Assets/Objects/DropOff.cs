using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DropOff : MonoBehaviour
{
    [SerializeField] private BoxCollider2D bounds;

    private UnityAction onSubmit;
    private GameObject item = null;

    private bool open;
    private float current;
    [SerializeField] private float rounding;
    [SerializeField] private float target;
    private float inPos;
    [SerializeField] private float speed;

    void Start()
    {
        inPos = transform.position.x;
        target -= inPos;
        float offset = target % rounding;
        target += offset+rounding;
        inPos += offset;
    }

    void Update()
    {
        current = Mathf.MoveTowards(current, open ? target : 0, Time.deltaTime * speed);
        if (item && current == 0)
        {
            onSubmit.Invoke();
            Destroy(item);
        }
        transform.position = new Vector3(inPos + Mathf.Floor(current/rounding)*rounding, transform.position.y, transform.position.z);
    }

    public void Submit(GameObject item, UnityAction onSubmit)
    {
        this.onSubmit = onSubmit;
        this.item = item;
        item.transform.parent = transform;
        open = false;
    }

    public bool IsHovered(Vector2 mousePos)
    {
        if (!open)
        {
            return false;
        }

        Bounds bounds = this.bounds.bounds;
        return mousePos.x >= bounds.min.x && mousePos.y >= bounds.min.y && mousePos.x <= bounds.max.x && mousePos.y <= bounds.max.y;
    }

    public void SetOpen(bool to)
    {
        if (item)
        {
            return;
        }
        open = to;
    }
}
