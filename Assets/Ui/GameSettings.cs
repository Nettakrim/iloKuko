using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSettings : MonoBehaviour
{
    public void ToggleFullscreen()
    {
        bool to = !Screen.fullScreen;
        if (to)
        {
            Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, FullScreenMode.FullScreenWindow);
        }
        else
        {
            Screen.SetResolution(384, 216, FullScreenMode.Windowed);
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
