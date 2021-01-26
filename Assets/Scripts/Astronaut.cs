using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Astronaut : Enemy
{
    [SerializeField]
    private float rightCap;
    [SerializeField]
    private float leftCap;
    [SerializeField]
    private float jumpHeight;
    [SerializeField]
    private float jumpLength;
    [SerializeField]
    private LayerMask ground;

    private Collider2D coll;

    private bool facingLeft = true;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        coll = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();
    }


    // Update is called once per frame
    void Update()
    {
        if (anim.GetBool("Jumping_State"))
        {
            if (rb.velocity.y < .1f)
            {
                anim.SetBool("Falling_State", true);
                anim.SetBool("Jumping_State", false);
            }
        }
        else if (coll.IsTouchingLayers(ground) && anim.GetBool("Falling_State"))
        {
            anim.SetBool("Jumping_State", false);
            anim.SetBool("Falling_State", false);
        }
    }

    private void Move()
    {
        if (facingLeft)
        {
            if (transform.position.x > leftCap)
            {
                if (transform.localScale.x != -1)
                {
                    transform.localScale = new Vector3(-1, 1);
                }

                if (coll.IsTouchingLayers(ground))
                {
                    rb.velocity = new Vector2(-jumpLength, jumpHeight);
                    anim.SetBool("Jumping_State", true);
                }
            }
            else
            {
                facingLeft = false;
            }
        }
        else
        {
            if (transform.position.x < rightCap)
            {
                if (transform.localScale.x != 1)
                {
                    transform.localScale = new Vector3(1, 1);
                }

                if (coll.IsTouchingLayers(ground))
                {
                    rb.velocity = new Vector2(jumpLength, jumpHeight);
                    anim.SetBool("Jumping_State", true);
                }
            }
            else
            {
                facingLeft = true;
            }
        }
    }
}
