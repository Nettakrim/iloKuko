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
        if (text == null)
        {
            text = GetComponent<TextMeshProUGUI>();
            lasina = text.text;
            pona = Global.instance.ConvertToSitelenPona(lasina);
        }
        TextManager.RegisterSitelenChange(UpdateText);
    }

    public void SetMessage(string message, string start)
    {
        text = GetComponent<TextMeshProUGUI>();
        lasina = start + message;
        pona = start + Global.instance.ConvertToSitelenPona(message);
        TextManager.RegisterSitelenChange(UpdateText);
    }

    private void UpdateText(bool useSitelenPona)
    {
        text.text = useSitelenPona ? pona : lasina;
    }

    public TextMeshProUGUI GetText()
    {
        return text;
    }
}
