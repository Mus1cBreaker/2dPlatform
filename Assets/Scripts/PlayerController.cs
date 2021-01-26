using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{

    private Rigidbody2D rb;
    private Animator anim;
    private Collider2D coll;
    private AudioSource jumpSound;

    private enum State { idle, running, jumping, falling, hurt } // Enumerator state
    private State state = State.idle;

    [SerializeField]
    private LayerMask ground;
    [SerializeField]
    private float speed = 5f;
    [SerializeField]
    private float jumpforce = 10f;
    [SerializeField]
    private float hurtForce = 5f;
    [SerializeField]
    public float RotationDuration = 0.5f;
    [SerializeField]
    public float zRotation = -90f;
    [SerializeField]
    public bool _isRotating = false;

    [SerializeField]
    public int lives = 5;
    [SerializeField]
    public Text AmountOfLives;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        coll = GetComponent<Collider2D>();
        jumpSound = GetComponent<AudioSource>();
        AmountOfLives.text = lives.ToString();
    }

    // Update is called once per frame
    void Update()
    {

        if (state != State.hurt)
        {
            Movement();
        }

        VelocityStateSwitch();
        anim.SetInteger("state", (int)state); // Set an animation
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag.Equals("Enemy"))
        {
            Enemy enemy = collision.gameObject.GetComponent<Enemy>();

            if (state == State.falling)
            {
                enemy.JumpedOn();
                Jump();
            }
            else
            {
                state = State.hurt;
                if (collision.gameObject.transform.position.x > transform.position.x)
                {
                    // Enemy is to the right
                    rb.velocity = new Vector2(-hurtForce, rb.velocity.y);
                }
                else
                {
                    // Enemy is to the left
                    rb.velocity = new Vector2(hurtForce, rb.velocity.y);
                }
                LivesManager();
            }
        }
    }

    private void LivesManager()
    {
        if (state == State.hurt)
        {
            lives -= 1;
            AmountOfLives.text = lives.ToString();
            if (lives <= 0)
            {
                SceneManager.LoadScene("First Level");
                lives = 5;
            }
        }
    }

    private void Movement()
    {
        float horDirection = Input.GetAxis("Horizontal");

        // Right movement
        if (horDirection > 0)
        {
            rb.velocity = new Vector2(speed, rb.velocity.y);
            transform.localScale = new Vector2(1, 1);
        }
        // Left movement
        else if (horDirection < 0)
        {
            rb.velocity = new Vector2(-speed, rb.velocity.y);
            transform.localScale = new Vector2(-1, 1);
        }
        // Jumping
        if (Input.GetButtonDown("Jump") && coll.IsTouchingLayers(ground))
        {
            Jump();
        }
    }

    private void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpforce);
        state = State.jumping;
        //Player starts to rotate
        StartCoroutine(RotatePlayer(
          transform.rotation,
          //Player is rotating on the Z-Axis
          transform.rotation * Quaternion.Euler(0, 0, zRotation),
          RotationDuration
        ));
    }

    private void VelocityStateSwitch()
    {
        if (state == State.jumping)
        {
            if (rb.velocity.y < .1f)
            {
                state = State.falling;
            }
        }
        else if (state == State.falling)
        {
            if (coll.IsTouchingLayers(ground))
            {
                state = State.idle;
            }
        }
        else if (state == State.hurt)
        {
            if (Mathf.Abs(rb.velocity.x) < .1f)
            {
                state = State.idle;
            }
        }
        else if (Mathf.Abs(rb.velocity.x) > 2f)
        {
            state = State.running;
        }
        else if (rb.velocity.y < .1f && !coll.IsTouchingLayers(ground))
        {
            state = State.falling;
        }
        else
        {
            state = State.idle;
        }
    }

    private void JumpSound()
    {
        jumpSound.Play();
    }

    IEnumerator RotatePlayer(Quaternion start, Quaternion end, float duration)
    {
        JumpSound();
        float horDirection = Input.GetAxis("Horizontal");
        float endTime = Time.time + duration;
        //Make sure that the script knows that the player is currently rotating
        _isRotating = true;
        while (Time.time <= endTime)
        {
            //Normalized progress from 0 - 1
            float t = 1f - (endTime - Time.time) / duration;
            if (horDirection > 0)
            {
                transform.rotation = Quaternion.Lerp(start, end, t);
            }
            else
            {
                transform.rotation = Quaternion.Lerp(end, start, t);
            }
            yield return 0;
        }
        transform.rotation = end;
        _isRotating = false;
    }
}
