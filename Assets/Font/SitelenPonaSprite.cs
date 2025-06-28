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
    }

    void Update()
    {
        spriteRenderer.sprite = TextManager.useSitelenPona ? pona : lasina;
    }
}
