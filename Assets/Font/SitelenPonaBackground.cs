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
        TextManager.RegisterSitelenChange(UpdateSize);
    }

    void UpdateSize(bool useSitelenPona)
    {
        (transform as RectTransform).sizeDelta = useSitelenPona ? ponaSize : lasinaSize;
    }

    public void Set(Vector2 lasinaSize, Vector2 ponaSize)
    {
        this.lasinaSize = lasinaSize;
        this.ponaSize = ponaSize;
        TextManager.RegisterSitelenChange(UpdateSize);
    }
}
