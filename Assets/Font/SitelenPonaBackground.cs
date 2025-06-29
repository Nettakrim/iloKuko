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
}
