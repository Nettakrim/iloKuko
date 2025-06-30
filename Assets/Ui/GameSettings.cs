using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameSettings : MonoBehaviour
{
    [SerializeField] private SitelenPonaImage fullscreen;
    [SerializeField] private SitelenPonaBackground fullscreenSize;
    [SerializeField] private Sprite[] fullscreenSprites;
    [SerializeField] private GameObject quitPrompt;
    [SerializeField] private GameSlider sfxVolume;
    [SerializeField] private GameSlider musicVolume;

    [SerializeField] private GameObject login;
    [SerializeField] private GameObject buttons;
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject cyberspace;
    [SerializeField] private GameObject tutorial;

    [SerializeField] private Image background;
    [SerializeField] private Sprite cyberBackground;
    [SerializeField] private GameObject waso;
    [SerializeField] private GameObject cyberWaso;
    [SerializeField] private GameObject recording;

    [SerializeField] private TokiInterpreter interpreter;

    [SerializeField] private SoundGroup startup;

    private static bool initialised = false;

    void Start()
    {
        UpdateFullscreenButtons(Screen.fullScreen);

        if (initialised)
        {
            Login();
        }
        else
        {
            login.SetActive(true);
            startup.Play(true);
            initialised = true;

            sfxVolume.SetValue(PlayerPrefs.GetFloat("SFX", 0.6666f));
            musicVolume.SetValue(PlayerPrefs.GetFloat("Music", 0.6666f));
        }
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

    public void SetSFXVolume(float to)
    {
        AudioManager.instance.SetVolume("SFX", to);
        PlayerPrefs.SetFloat("SFX", to);
    }

    public void SetMusicVolume(float to)
    {
        AudioManager.instance.SetVolume("Music", to);
        PlayerPrefs.SetFloat("Music", to);
    }

    public void Login()
    {
        login.SetActive(false);
        buttons.SetActive(true);
        mainMenu.SetActive(true);
        Destroy(login);
    }

    public void Quit()
    {
        initialised = false;
        Global.ignoreMask = 0;
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }

    public void FullQuit()
    {
        Application.Quit();
    }

    public void ToggleQuitPrompt()
    {
        quitPrompt.SetActive(!quitPrompt.activeSelf);
        Global.ignoreMask = (Global.ignoreMask & (~2)) | (quitPrompt.activeSelf ? 2 : 0);
        Time.timeScale = quitPrompt.activeSelf ? 0 : 1;
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

        Destroy(mainMenu);
    }

    public void LoadMain()
    {
        mainMenu.SetActive(false);
        cyberspace.SetActive(true);
        interpreter.SetInterpreter("start");

        Destroy(tutorial);
        Destroy(mainMenu);
    }
}
