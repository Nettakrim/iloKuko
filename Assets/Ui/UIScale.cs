using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIScale : MonoBehaviour
{
    [SerializeField]
    private CanvasScaler canvasScaler;

    private float threshold = 16f / 9f;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        canvasScaler.matchWidthOrHeight = (Screen.width / (float)Screen.height) > threshold ? 1 : 0;
    }
}
