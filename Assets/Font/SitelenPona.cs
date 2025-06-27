using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class SitelenPona : MonoBehaviour
{
    private TextMeshProUGUI text;
    private string lasina;
    private string pona;

    void Start()
    {
        if (text == null) {
            text = GetComponent<TextMeshProUGUI>();
            lasina = text.text;
            pona = Global.instance.ConvertToSitelenPona(lasina);
        }
    }

    void Update()
    {
        UpdateText();
    }

    public void SetMessage(string message) {
        text = GetComponent<TextMeshProUGUI>();
        lasina = message;
        pona = Global.instance.ConvertToSitelenPona(lasina);
        UpdateText();
    }

    private void UpdateText() {
        text.text = TextManager.useSitelenPona ? pona : lasina;
    }
}
