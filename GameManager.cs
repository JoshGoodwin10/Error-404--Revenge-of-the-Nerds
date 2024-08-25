using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject pauseMenu;
    public GameObject HUD;
    public bool isPaused;
    public TMP_Text killCount;
    public TMP_Text waveIndicatorText;

    private float numberKills;
    private float enemiesLeft;
    private float currentWave;
    // Start is called before the first frame update
    void Start()
    {
        isPaused = false;
        numberKills = 0;
        enemiesLeft = 10;
        currentWave = 1;

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                resumeGame();
            }

            else
            {
                pauseGame();
            }
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            enemyKilled();
        }
    }

    public void pauseGame()
    {
        isPaused = true;
        HUD.SetActive(false);
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;
        
    }

    public void resumeGame()
    {
        isPaused = false;
        HUD.SetActive(true);
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
    }

    public void toMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Main Menu");
    }

    public void quitGame()
    {
        Application.Quit();
    }

    public void enemyKilled()
    {
        numberKills++;
        enemiesLeft--;
        killCount.text = "" + numberKills;
        isWaveFinished();
    }

    public bool isWaveFinished()
    {
        if (enemiesLeft == 0)
        {
            currentWave++;
            waveIndicatorText.text = "Wave " + currentWave;
            enemiesLeft = 10;
            return true;
        }

        else
            return false;    
    }
}
