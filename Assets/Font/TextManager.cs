using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextManager : MonoBehaviour
{
    public static bool useSitelenPona;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S)) {
            useSitelenPona = !useSitelenPona;
        }
    }
}
