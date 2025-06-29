using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SitelenPonaBackground : MonoBehaviour
{
    [SerializeField] private Vector2 ponaSize;
    private Vector2 lasinaSize;

    void Start()
    {
        lasinaSize = (transform as RectTransform).sizeDelta;
    }

    void Update()
    {
        (transform as RectTransform).sizeDelta = TextManager.useSitelenPona ? ponaSize : lasinaSize;
    }

    public void Set(Vector2 lasinaSize, Vector2 ponaSize)
    {
        this.lasinaSize = lasinaSize;
        this.ponaSize = ponaSize;
    }
}
