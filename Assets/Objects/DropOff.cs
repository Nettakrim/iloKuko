using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DropOff : MonoBehaviour
{
    [SerializeField] private BoxCollider2D bounds;

    private Item item = null;
    private Box box = null;

    private bool isOpen;
    private float current;
    [SerializeField] private float rounding;
    [SerializeField] private float target;
    private float inPos;
    [SerializeField] private float speed;

    [SerializeField] private SoundGroup open;
    [SerializeField] private SoundGroup close;
    [SerializeField] private SoundGroup accept;
    [SerializeField] private SoundGroup reject;

    void Start()
    {
        inPos = transform.position.x;
        target -= inPos;
        float offset = target % rounding;
        target += offset + rounding;
        inPos += offset;
    }

    void Update()
    {
        current = Mathf.MoveTowards(current, isOpen ? target : 0, Time.deltaTime * speed);
        if (item)
        {
            if (current == 0)
            {
                box.OnSubmit(item);
                if (!isOpen)
                {
                    Destroy(item.gameObject);
                    accept.Play(true);
                }
                else
                {
                    reject.Play(true);
                }
            }
            if (current == target)
            {
                item.transform.SetParent(box.transform);
                box.ItemRejected(item);
                item = null;
            }
        }
        transform.position = new Vector3(inPos + Mathf.Floor(current / rounding) * rounding, transform.position.y, transform.position.z);
    }

    public void Submit(Item item, Box box)
    {
        this.item = item;
        this.box = box;
        item.transform.parent = transform;
        isOpen = false;
    }

    public bool IsHovered(Vector2 mousePos)
    {
        if (!isOpen || item)
        {
            return false;
        }

        Bounds bounds = this.bounds.bounds;
        return mousePos.x >= bounds.min.x && mousePos.y >= bounds.min.y && mousePos.x <= bounds.max.x && mousePos.y <= bounds.max.y;
    }

    public void SetOpen(bool to)
    {
        if (isOpen == to || item)
        {
            return;
        }
        isOpen = to;
        (isOpen ? open : close).Play(false);
    }

    public void Reject()
    {
        isOpen = true;
        open.Play(false);
    }
}
