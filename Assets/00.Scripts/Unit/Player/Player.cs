using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float acceleration = 50f;
    [SerializeField] private float decceleration = 40f;
    [SerializeField] private float velPower = 0.9f;
    [SerializeField] private float maxSpeed = 13f;

    [Header("Jump")]
    [SerializeField] private float jumpPower = 6f;
    [SerializeField] private float fallMultiplier = 2.5f; // 떨어질 때 중력 배율
    [SerializeField] private float lowJumpMultiplier = 2f; // 점프 끝날 때 가속
    private bool isJump = true;

    public Gun[] guns;
    private bool isAttack = true;
    private float time;
    private int cureentGun = 0;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Animator animator;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = rb.GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        Jump();
        if(Input.GetMouseButtonDown(0) & isAttack)
        {
            StartCoroutine(Attack());
        }
    }

    private void FixedUpdate()
    {
        Move();

    }
    private void Move()
    {
        float x = Input.GetAxisRaw("Horizontal");

        if (x != 0)
        {
            //animator.SetTrigger("move");
            spriteRenderer.flipX = x < 0;
        }

        float targetSpeed = x * moveSpeed;
        float speedDif = targetSpeed - rb.velocity.x;

        float accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? acceleration : decceleration;

        float movement = Mathf.Pow(Mathf.Abs(speedDif) * accelRate, velPower) * Mathf.Sign(speedDif);

        rb.AddForce(movement * Vector2.right);

        if (Mathf.Abs(rb.velocity.x) > maxSpeed)
        {
            rb.velocity = new Vector2(Mathf.Sign(rb.velocity.x) * maxSpeed, rb.velocity.y);
        }

    }

    private void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isJump)
        {
            isJump = false;
            rb.velocity = new Vector2(rb.velocity.x, jumpPower);
        }

        if (rb.velocity.y < 0) 
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }
      
    }

    private IEnumerator Attack()
    {
        float time = guns[cureentGun].attackSpeed;
        isAttack = false;
        guns[cureentGun].Fire();
        yield return new WaitForSeconds(time);
        isAttack = true;


    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.collider.CompareTag("Ground"))
        {
            isJump=true;
        }
    }
}
