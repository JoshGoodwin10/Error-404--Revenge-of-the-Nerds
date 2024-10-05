using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using TMPro;
using TMPro.Examples;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("General Enemy Spawning ")]
    public KeyCode spawnKey = KeyCode.N;
    public GameObject enemyPrefab;
    public int spawnDistance = 100;
    private bool isSpawning = false;


    [Header("Group Spawning")]
    public int groupSize = 5; 
    public float increasingFactor = 1;
    public bool increasingEnemyGroupCount = false;

    [Header("Game Mode")]
    public bool zombies = false;
    public bool spawnPerClick = false;



    // Zombie Wave System 
    private int currentWave = 0;
    private GameObject waveCountObject;
    private TMP_Text waveTextCountIndicator;
    public int remainingEnemies = 0; 
    


    void Start()
    {
        remainingEnemies = GameObject.FindGameObjectsWithTag("Enemy").Length;

        if (zombies)
        {
            waveCountObject = GameObject.Find("Wave Indicator");
            waveTextCountIndicator = waveCountObject.GetComponent<TMP_Text>();
            waveTextCountIndicator.text = "Wave " + currentWave.ToString();
        }
    }

    void Update()
    {
        accessInput();

        if (spawnPerClick&& isSpawning) {
            createEnemyGroup();
            isSpawning = false;
        }
        if (zombies)
        {
            createNextZombieWave(); 
        }

            
    }

  
    void accessInput()
    {
        if (Input.GetKeyDown(spawnKey))
        {
            isSpawning = true;
        }
    }

    private void createEnemy()
    {
        GameObject newEnemy = Instantiate(enemyPrefab,transform.position, Quaternion.identity);
        Vector3 enemyPosition= new Vector3(Random.Range(-spawnDistance, spawnDistance),0,  Random.Range(-spawnDistance, spawnDistance)) + transform.position ;
        newEnemy.transform.localPosition = enemyPosition;

        newEnemy.transform.localRotation = Quaternion.identity;
        newEnemy.transform.localScale = new Vector3(1, 1, 1);
        remainingEnemies++;
    }
    private void increaseGroupSize()
    {
        groupSize = (int)(groupSize * increasingFactor);
    }

    private void createEnemyGroup()
    {
        for (int i = 0; i < groupSize; i++)
        {
            createEnemy();
        }
        
        if (increasingEnemyGroupCount)
        {
            increaseGroupSize();
        }
    }
    
    private void createNextZombieWave()
    {
        remainingEnemies = GameObject.FindGameObjectsWithTag("Enemy").Length;
        if( remainingEnemies == 0)
        {
            createEnemyGroup();
            currentWave++;
            waveTextCountIndicator.text = "Wave " + currentWave.ToString();
        }

    }


}
