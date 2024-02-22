using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float playerSpeed;
    public Rigidbody2D rb;
    public GameObject rock;
    public GameObject leftBoundary;
    public GameObject rightBoundary;
    public Animator animator;
    public GameController gameController;

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
        if (Input.GetKeyDown("escape"))
        {
            Application.Quit();
        }

        if (!gameController.gamePaused)
        {
            Move();

            if (Input.GetKeyDown("space") && !gameController.RockInPlay())
            {
                ThrowRocks();
            }
        }
    }

    void Move()
    {
        float x = Input.GetAxis("Horizontal") * playerSpeed;

//TODO: Shouldn't be modifying velocity directly. Apply force instead, if time permits
        rb.velocity = new Vector2(x, 0.0f);
        rb.position = new Vector3(Mathf.Clamp(rb.position.x, 
                                                leftBoundary.transform.position.x, 
                                                rightBoundary.transform.position.x),
                                  rb.position.y,
                                  0.0f);
    }

    void ThrowRocks()
    {
        gameController.IncreaseRockCount(); 
        audioManager.PlaySFX(audioManager.Playerthrow);
        animator.Play("Throw");

        Instantiate(rock, rb.position, Quaternion.identity);
    }

    public void IncreaseSpeed()
    {
        playerSpeed += 0.2f;
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
