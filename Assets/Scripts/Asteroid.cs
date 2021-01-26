using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : Enemy
{
    [SerializeField]
    private float upCap;
    [SerializeField]
    private float downCap;
    [SerializeField]
    private float asteroidSpeed = 5;

    private bool facingDown = true;

    protected override void Start()
    {
        base.Start();
        rb = GetComponent<Rigidbody2D>();
    }


    // Update is called once per frame
    void Update()
    {
        Move();
    }

    private void Move()
    {
        if (facingDown)
        {
            if (transform.position.y > downCap)
            {
                rb.velocity = new Vector2(0, -asteroidSpeed);
            }
            else
            {
                facingDown = false;
            }
        }
        else
        {
            if (transform.position.y < upCap)
            {
                rb.velocity = new Vector2(0, asteroidSpeed);
            }
            else
            {
                facingDown = true;
            }
        }
    }
}
