using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public static Player Instance;

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

    [Header("Setting")]
    [SerializeField] private float curHp;
    [SerializeField] private float maxHp;

    public float invincibleTime = 1.5f;     // 무적 유지 시간
    public float blinkInterval = 0.6f;    // 깜빡임 간격

    private bool isInvincible = false;    // 현재 무적인지 여부
    [SerializeField] private Image hitEffect;

    public Gun[] guns;
    private bool isAttack = true;
    private float time;
    private int cureentGun = 0;


    private Rigidbody2D rb;
    public SpriteRenderer spriteRenderer;
    private Animator animator;

    #region 프로피터
    public float CurHp => curHp;
    public float MaxHp => maxHp;
    #endregion
    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = rb.GetComponent<SpriteRenderer>();
        curHp = maxHp;
    }

    // Update is called once per frame
    void Update()
    {
        Jump();
        if(Input.GetMouseButtonDown(0) & isAttack)
        {
            StartCoroutine(Attack());
        }

        guns[cureentGun].gameObject.GetComponent<SpriteRenderer>().flipX = spriteRenderer.flipX ? false : true;
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
        CameraShake.instance.OnShakeCamera(0.25f, 0.05f);
        yield return new WaitForSeconds(time);
        isAttack = true;


    }

    public void TakeDamage(float damage)
    {
        if(isInvincible == true)
        {
            return;
        }
        curHp -= damage;
        StartCoroutine(HitEffect());
        StartCoroutine(InvincibleBlink());
        Debug.Log("피격 당함");

        if (curHp <= 0)
        {
            Die();
        }
    }

    private IEnumerator InvincibleBlink()
    {
        isInvincible = true;
        float elapsed = 0f;

        while (elapsed < invincibleTime)
        {
            spriteRenderer.enabled = false; // 스프라이트 끄기
            guns[cureentGun].GetComponent<SpriteRenderer>().enabled = false;
            yield return new WaitForSeconds(blinkInterval / 2);

            spriteRenderer.enabled = true; // 스프라이트 켜기
            guns[cureentGun].GetComponent<SpriteRenderer>().enabled = true;
            yield return new WaitForSeconds(blinkInterval / 2);

            elapsed += blinkInterval;
        }

        isInvincible = false; // 무적 해제
        spriteRenderer.enabled = true; // 반드시 켜놓기
        guns[cureentGun].GetComponent<SpriteRenderer>().enabled = true;
    }

    private IEnumerator HitEffect()
    {
        Color color = hitEffect.color;
        color.a = 0.4f;
        hitEffect.color = color;

        while (color.a >= 0.0f)
        {
            color.a -= Time.deltaTime;
            hitEffect.color = color;

            yield return null;
        }

    }

    private void Die()
    {
        Debug.Log("플레이어 죽음");
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.collider.CompareTag("Ground"))
        {
            isJump=true;
        }
    }
}
