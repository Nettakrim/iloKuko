using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class SitelenPona : MonoBehaviour
{
    TextMeshProUGUI text;
    string lasina;
    string pona;

    void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
        lasina = text.text;
        pona = Global.instance.ConvertToSitelenPona(lasina);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            text.text = lasina;
        } else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            text.text = pona;
        }
    }
}
