using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SitelenPonaSprite : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite lasina;
    [SerializeField] private Sprite pona;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        TextManager.RegisterSitelenChange(UpdateSprite);
    }

    void UpdateSprite(bool useSitelenPona)
    {
        spriteRenderer.sprite = useSitelenPona ? pona : lasina;
    }
}
