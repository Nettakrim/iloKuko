using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SitelenPonaImage : MonoBehaviour
{
    private Image image;
    [SerializeField] private Sprite lasina;
    [SerializeField] private Sprite pona;

    void Start()
    {
        image = GetComponent<Image>();
    }

    void Update()
    {
        image.sprite = TextManager.useSitelenPona ? pona : lasina;
    }

    public void Set(Sprite lasina, Sprite pona)
    {
        this.lasina = lasina;
        this.pona = pona;
    }
}
