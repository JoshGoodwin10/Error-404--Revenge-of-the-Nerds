using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//using UnityEngine;
//using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Linq;
using UnityEngine.Audio;
using System;

public class StartGame : MonoBehaviour
{
    public GameObject settingsMenu;
    public GameObject buttonsMenu;
    public GameObject modesMenu;
    public TMP_Text PlayGame;
    public TMP_Text Settings;
    public TMP_Text QuitGame;
    public TMP_Text DeathMatch;
    public TMP_Text Waves;
    public TMP_Text FreeForAll;
    public TMP_Text CloseOptions;
    public AudioMixer audioMixer;
    public TMP_Dropdown resolutionDropdown;

    Resolution[] resolutions;
    void Start()
    {
        resolutions = Screen.resolutions;

        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();

        int currentResolutionIndex = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);

            if (resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }
        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();

        PlayGame.color = new Color(150, 150, 150, 255);
        Settings.color = new Color(150, 150, 150, 255);
        QuitGame.color = new Color(150, 150, 150, 255);
        DeathMatch.color = new Color(150, 150, 150, 255);
        Waves.color = new Color(150, 150, 150, 255);
        FreeForAll.color = new Color(150, 150, 150, 255);
        CloseOptions.color = new Color(150, 150, 150, 255);
    }

    public void selectMode()
    {
        buttonsMenu.SetActive(false);
        modesMenu.SetActive(true);
    }

    public void closeSelectMode()
    {
        buttonsMenu.SetActive(true);
        modesMenu.SetActive(false);
    }

    public void Play()
    {
        SceneManager.LoadScene("Main Game");
    }

    public void ShowOptions()
    {
        buttonsMenu.SetActive(false);
        settingsMenu.SetActive(true);
    }

    public void HideOptions()
    {
        buttonsMenu.SetActive(true);
        settingsMenu.SetActive(false);
    }

    public void Quit()
    {
        Application.Quit();
    }

    // Settings 
    public void setVolume(float volume)
    {
        audioMixer.SetFloat("Volume", volume);
    }

    public void setFullScreen(bool isFullScreen)
    {
        Screen.fullScreen = isFullScreen;
    }

    public void setResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }
}
