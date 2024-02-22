using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public Player player;
    public GameObject greenGoblin;
    public GameObject hobgoblin;
    public GameObject redGoblin;
    public GameObject superGoblin;
    public GameObject leftBoundary;
    public GameObject rightBoundary;
    public GameObject bottomBoundary;
    public GameObject gameOverPanel;
    public GameObject shopPanel;
    public GameObject buyHealth;
    public GameObject buySpeed;
    public GameObject shopAnnouncementText;
    public GameObject shopInfoText;
    public GameObject instructionsPanel;

    public TMP_Text healthText;
    public TMP_Text scoreText;
    public TMP_Text healthPriceText;
    public TMP_Text speedPriceText;
    public TMP_Text levelText;

    public GameObject skyObject;
    private SpriteRenderer skySpriteRenderer;
    public Sprite[] skies;

    public bool gameOver = false;
    public bool gamePaused = true;
    public int healthCost;
    public int speedCost;
    public int health;
//FIXME: these two vars need better names lmao
    public int healthItemHealAmount;
    public int speedItemIncAmount;
    public int priceIncFactor = 2;
    public int goblinCount = 0;

    private int coins = 0;
    private bool shopAvailable = false;
    private bool inShop = false;
    private int goblinsSpawned = 0;
    private int spawnerTarget = 10;
    private IEnumerator enemySpawner;
    private int rockCount = 0;

    /***************** level stuff ****************/
    private int level = 0;
    private int levelCount = 3;
    // Determines the spawn rates at each level
    //      green    hob     red = 1 - (green + hob)
    private float[,] levelOdds =
    {
        {   0.4f,   0.6f},  
        {   0.4f,   0.4f},  
        {   0.333f, 0.333f} 
    };

    AudioManager audioManager;

    void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }
    
    void Start()
    {
        gameOverPanel.SetActive(false);
        shopPanel.SetActive(false);
        shopAnnouncementText.SetActive(false);
        shopInfoText.SetActive(false);
        instructionsPanel.SetActive(true);

        // have to do this in order to stop the coroutine later
        enemySpawner = SpawnGoblins();

        skySpriteRenderer = skyObject.GetComponent<SpriteRenderer>();
        skySpriteRenderer.sprite = skies[0];
    }

    void Update()
    {
        if (!gamePaused)
        {
            if (health <= 0)
            {
                gameOver = true;
                StartCoroutine(GameOverSequence());
            }

            if (goblinsSpawned >= spawnerTarget && goblinCount == 0 && !inShop)
            {
                EndLevel();
            }

            if (shopAvailable && Input.GetKeyDown("tab"))
            {
                SetupShop();
            }

            if (inShop)
            {
                DisplayShop();
            }

            if (inShop && Input.GetKeyDown("q"))
            {
                StartNextLevel();
            }
        }
        else if (Input.GetKeyDown("q"))
        {
            instructionsPanel.SetActive(false);
            gamePaused = false;
            StartCoroutine(enemySpawner);
        }

        UpdateScoreText();
        UpdateHealthText();
        UpdateLevelText();


        skySpriteRenderer.sprite = skies[level < skies.Length ? level : skies.Length - 1];
    }

    IEnumerator SpawnGoblins()
    {
        while (!gameOver && goblinsSpawned < spawnerTarget)
        {
            GameObject goblin = SelectGoblin();

            goblinsSpawned++;       // tracks # of CREATED goblins
            IncreaseGoblinCount();  // tracks # of ALIVE goblins

            Vector3 spawnLocation = new Vector3(Random.Range(leftBoundary.transform.position.x, rightBoundary.transform.position.x),
                                                bottomBoundary.transform.position.y,
                                                0.0f);
            Instantiate(goblin, spawnLocation, Quaternion.identity);

            yield return new WaitForSeconds(Random.Range((1.0f / (level + 1)), 2.0f));
        }
    }

    private GameObject SelectGoblin()
    {
        if (level == 0)
        {
            return greenGoblin;
        }

        float tempVal = Random.Range(0.0f, 1.0f);
        int i = level < levelCount ? level - 1 : levelCount - 1;

        if (tempVal < levelOdds[i, 0])
        {
            return greenGoblin;
        }
        else if (tempVal < levelOdds[i, 0] + levelOdds[i, 1])
        {
            return hobgoblin;
        }
        
        if (level > levelCount + 1)
        {
            int coinToss = Random.Range(0, 2); // Random.Range is [,) with ints for some reason

            if (coinToss == 1)
            {
                return superGoblin;
            }

            return redGoblin;
        }

        return redGoblin;
    }

    public void IncreaseGoblinCount()
    {
        goblinCount++;
    }

    private IEnumerator GameOverSequence()
    {
        gameOverPanel.SetActive(true);

        while (gameOver)
        {
            if (Input.GetKeyDown("q"))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }

            yield return new WaitForSeconds(1.0f);
        }
    }

    private void EndLevel()
    {
        StopCoroutine(enemySpawner);
        shopAvailable = true;
        shopAnnouncementText.SetActive(true);
    }

    private void SetupShop()
    {
        audioManager.PlayShopMusic();
        shopAnnouncementText.SetActive(false);
        shopInfoText.SetActive(true);
        inShop = true;
        shopAvailable = false;
    }

    private void DisplayShop()
    {
        healthPriceText.text = healthCost.ToString();
        speedPriceText.text = speedCost.ToString();

        shopPanel.SetActive(true);

        if (coins < healthCost)
        {
            buyHealth.SetActive(false);
        }
        else
        {
            buyHealth.SetActive(true);
        }

        if (coins < speedCost)
        {
            buySpeed.SetActive(false);
        }
        else
        {
            buySpeed.SetActive(true);
        }
    }

    private void StartNextLevel()
    {
        level++;
        goblinsSpawned = 0;

        spawnerTarget += 5;

        shopInfoText.SetActive(false);
        shopAvailable = false;
        shopPanel.SetActive(false);

        // I have absolutely zero clue why I have to 
        // reassign enemySpawner to SpawnGoblins,
        // but coroutine won't restart without it.
        enemySpawner = SpawnGoblins();
        StartCoroutine(enemySpawner);

        audioManager.StopShopMusic();
        audioManager.PlayBackgroundMusic();

        inShop = false;
    }

    public void IncreaseCoins()
    {
        coins++;
    }

    public void BonusPoints()
    {
        coins += 2;
    }

    private void UpdateScoreText()
    {
        scoreText.text = coins.ToString();
    }

    public void IncreaseHealth()
    {
        health += healthItemHealAmount;
        coins -= healthCost;
        healthCost = (int)(healthCost * priceIncFactor);
    }

    public void DoDamage(int damageAmount)
    {
        health -= damageAmount;
    }

    private void UpdateHealthText()
    {
        healthText.text = health.ToString();
    }

    private void UpdateLevelText()
    {
        levelText.text = "Level: " + (level + 1);
    }

    public void IncreaseSpeed()
    {
        player.IncreaseSpeed();
        coins -= speedCost;
        speedCost = (int)(speedCost * priceIncFactor);
    }

    public void DecreaseGoblinCount()
    {
        goblinCount--;
    }

    public bool RockInPlay()
    {
        return rockCount == 1;
    }

    public void IncreaseRockCount()
    {
        rockCount++;
    }

    public void DecreaseRockCount()
    {
        rockCount--;
    }
}
