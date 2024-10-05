using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] public PlayerMovement PM;
    [SerializeField] public GameObject pauseMenu;
    public TokenManager TM;
    public GameObject HUD;
    public GameObject endMenu;
    public bool isPaused;
    public TMP_Text killCount;
    public TMP_Text waveIndicatorText;

    public float numberKills;
    private float enemiesLeft;
    private float currentWave;

    public EnemySpawner ES;
    // Start is called before the first frame update
    void Start()
    {

        Time.timeScale = 1f;

        isPaused = false;
        numberKills = 0;
        enemiesLeft = 10;
        currentWave = 1;

    }

    // Update is called once per frame
    void Update()
    {
        waveIndicatorText.text = "Wave " + currentWave;
        enemiesLeft = ES.remainingEnemies;
        Debug.Log("Left: " + enemiesLeft);

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

        if (PM.playerHealth <= 0)
        {
            endGame();
        }
        killCount.text = "" + numberKills;
    }

    public void pauseGame()
    {
        isPaused = true;
        HUD.SetActive(false);
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void resumeGame()
    {
        isPaused = false;
        HUD.SetActive(true);
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
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
        
        isWaveFinished();

        TM.AddTokens(50);
    }

    public bool isWaveFinished()
    {
        if (enemiesLeft == 0)
        {
            currentWave++;
            return true;
        }

        else
            return false;    
    }

    public void endGame()
    {
        HUD.SetActive(false);
        endMenu.SetActive(true);
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
