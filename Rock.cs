using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockBehavior : MonoBehaviour
{
    public Rigidbody2D rb;
    public GameObject rock;
    public GameController gameController;
    //public GameObject boundary;

    private float gravity = -9.8f;

    AudioManager audioManager;

    private void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        FindGameController();
    }

    void Update()
    {
        if (!gameController.gamePaused)
        {
            rb.velocity = new Vector2(0.0f, gravity);

            //FIXME: boundary ideally shouldn't be hardcoded in
            if (rb.position.y <= -6.0f) //boundary.transform.position.y)
            {
                gameController.DecreaseRockCount();
                Destroy(rock);
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "enemy")
        {
            audioManager.PlaySFX(audioManager.rock);
            audioManager.PlaySFX(audioManager.goblin);
            Goblin goblin = other.GetComponent<Goblin>();
            goblin.isFalling = true;
            gameController.IncreaseCoins();
            gameController.DecreaseRockCount();
            Destroy(rock);
        }
    }

//TODO: try creating utilities file for this
    void FindGameController()
    {
        GameObject gameControllerObject = GameObject.FindWithTag("GameController");
        if (gameControllerObject != null)
        {
            gameController = gameControllerObject.GetComponent<GameController>();
        }
        else
        {
            UnityEngine.Debug.Log("Cannot find 'GameController' script");
        }
        if (gameController == null)
        {
            UnityEngine.Debug.Log("Cannot find 'GameController' script");
        }
    }
}
