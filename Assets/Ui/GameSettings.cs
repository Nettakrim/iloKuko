using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameSettings : MonoBehaviour
{
    [SerializeField] private SitelenPonaImage fullscreen;
    [SerializeField] private SitelenPonaBackground fullscreenSize;
    [SerializeField] private Sprite[] fullscreenSprites;

    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject cyberspace;
    [SerializeField] private GameObject tutorial;

    [SerializeField] private Image background;
    [SerializeField] private Sprite cyberBackground;
    [SerializeField] private GameObject waso;
    [SerializeField] private GameObject cyberWaso;
    [SerializeField] private GameObject recording;

    [SerializeField] private TokiInterpreter interpreter;

    void Start()
    {
        mainMenu.SetActive(true);
        UpdateFullscreenButtons(Screen.fullScreen);
    }

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
        UpdateFullscreenButtons(to);
    }

    private void UpdateFullscreenButtons(bool to)
    {
        if (to)
        {
            fullscreen.Set(fullscreenSprites[1], fullscreenSprites[3]);
            fullscreenSize.Set(new Vector2(15, 9), new Vector2(9, 9));
        }
        else
        {
            fullscreen.Set(fullscreenSprites[0], fullscreenSprites[2]);
            fullscreenSize.Set(new Vector2(19, 9), new Vector2(9, 9));
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
        background.sprite = cyberBackground;
        waso.SetActive(false);
        cyberWaso.SetActive(true);
        recording.SetActive(false);
    }

    public void LoadMain()
    {
        mainMenu.SetActive(false);
        cyberspace.SetActive(true);
        interpreter.SetInterpreter("start");

        Destroy(tutorial);
    }
}
