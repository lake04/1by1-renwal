using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{


    private float moveSpeed =5f;
    private float jumpPower = 10f;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = rb.GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }

    private void Move()
    {
        float x = Input.GetAxisRaw("Horizontal");

        spriteRenderer.flipX = x <0 ? true : false;

        rb.velocity = new Vector2(x * moveSpeed, 0);
    }
}
