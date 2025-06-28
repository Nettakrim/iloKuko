using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameSettings : MonoBehaviour
{
    public Image fullscreen;
    public Sprite[] fullscreenSprites;

    public GameObject mainMenu;
    public GameObject cyberspace;

    void Start()
    {
        mainMenu.SetActive(true);
    }

    public void ToggleFullscreen()
    {
        bool to = !Screen.fullScreen;
        if (to)
        {
            fullscreen.sprite = fullscreenSprites[0];
            Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, FullScreenMode.FullScreenWindow);
        }
        else
        {
            fullscreen.sprite = fullscreenSprites[1];
            Screen.SetResolution(384, 216, FullScreenMode.Windowed);
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void LoadTutorial()
    {
        Debug.LogError("TODO!!!!");
    }

    public void LoadMain()
    {
        mainMenu.SetActive(false);
        cyberspace.SetActive(true);
    }
}
