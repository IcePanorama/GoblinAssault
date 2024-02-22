using System.Collections;
using System.Collections.Generic;
using System.Security;
using UnityEngine;

public class Goblin : MonoBehaviour
{
    public GameObject goblin;
    public GameController gameController;
    public float speed;
    public int damage;
    public bool isFalling = false;
    public bool isSuperGoblin;

    private Rigidbody2D rb;
    private GameObject leftBoundary;
    private GameObject rightBoundary;
    private float horSpeed;

    AudioManager audioManager;

//TODO: make gravity accessible to all scripts so we don't have duplicates.
    private float gravity = -9.8f;

    private void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        leftBoundary = GameObject.FindGameObjectWithTag("LeftBoundary");
        rightBoundary = GameObject.FindGameObjectWithTag("RightBoundary");

        horSpeed    = isSuperGoblin ? UnityEngine.Random.Range(0.1f, speed) : 0.0f;

        //TODO: move FindGameController to a utility file.
        FindGameController();
    }

    void Update()
    {
        if (!gameController.gamePaused)
        {
            if (isFalling)
            {
                rb.velocity = new Vector2(0.0f, gravity);
            }
            else if (!gameController.gameOver)
            {
                Move();
            }
            else
            {
                rb.velocity = new Vector2(0.0f, 0.0f);
            }

            //FIXME: ideally shouldn't be using magic number here
            if (isFalling && rb.position.y <= -6.0f)
            {
                Destroy(goblin);
                gameController.DecreaseGoblinCount();
            }
        }
    }

    private void Move()
    {
        if (rb.position.x <= leftBoundary.transform.position.x 
            || rb.position.x >= rightBoundary.transform.position.x)
        {
            horSpeed = -horSpeed;
        }

        rb.velocity = new Vector2(horSpeed, speed);
        
        //TODO: HANDLE ROTATION OF SPRITE

        /*UnityEngine.Debug.Log("Speed: " + speed + " HorSpeed: " + horSpeed + "\nraw: " + Mathf.Rad2Deg * Mathf.Tan(speed / horSpeed) + " clamped: " + Mathf.Clamp(Mathf.Rad2Deg * Mathf.Tan(speed / horSpeed), -90.0f, 90.0f));

        goblin.transform.rotation = new Quaternion(0.0f,
                                                0.0f,
                                                Mathf.Clamp(Mathf.Rad2Deg * Mathf.Tan(horSpeed / speed), -90.0f, 90.0f),
                                                0.0f); */
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "TopBoundary")
        {
            audioManager.PlaySFX(audioManager.hurt);
            gameController.DoDamage(damage);
            Destroy(goblin);
            gameController.DecreaseGoblinCount();
        }
        else if (other.tag == "enemy" && isFalling)
        {
            audioManager.PlaySFX(audioManager.goblin);

            Goblin otherGoblin = other.GetComponent<Goblin>();
            otherGoblin.isFalling = true;

            gameController.BonusPoints();
        }
    }

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
