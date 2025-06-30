using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class SitelenPonaMessage : SitelenPona
{
    protected override void UpdateText(bool useSitelenPona)
    {
        base.UpdateText(useSitelenPona);
        GetText().margin = new Vector4(useSitelenPona ? 2 : 0, 0, 0, 0);
    }
}
