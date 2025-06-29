using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameSettings : MonoBehaviour
{
    [SerializeField] private Image fullscreen;
    [SerializeField] private Sprite[] fullscreenSprites;

    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject cyberspace;
    [SerializeField] private GameObject tutorial;
    
    [SerializeField] private TokiInterpreter interpreter;

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
        mainMenu.SetActive(false);
        cyberspace.SetActive(true);
        interpreter.SetInterpreter("tutorial");

        tutorial.SetActive(true);
    }

    public void LoadMain()
    {
        mainMenu.SetActive(false);
        cyberspace.SetActive(true);
        interpreter.SetInterpreter("start");

        Destroy(tutorial);
    }
}
