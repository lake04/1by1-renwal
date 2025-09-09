using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
    public bool isMove = true;
    public bool isKonBack = false;

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

    [Header("Gun")]
    public List<Gun> guns = new List<Gun>();
    public Transform PistolSkillPos;
    private bool isAttack = true;
    private float time;
    public int cureentGun = 0;

    public List<GameObject> gunPoss;
    public GameObject curGun;

    [Header("Gun Slot")]
    [SerializeField] private List<Image> slots;

    public Rigidbody2D rb;
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
        animator = GetComponent<Animator>();
        foreach(var slot  in slots)
        {
            slot.enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        Jump();
        if(Input.GetKeyDown(KeyCode.C) & isAttack)
        {
            StartCoroutine(Attack());
        }
        if(Input.GetKeyDown(KeyCode.X))
        {
            if(guns[cureentGun] ==null)
            {
                return;
            }
            guns[cureentGun].Skill();
        }
        if (Input.GetKeyDown(KeyCode.G))
        {
            DropGun();
        }

        for (int i = 0; i <=guns.Count; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha0 + i))
            {
                ChageSlot(i-1);
            }
        }

    }

    private void FixedUpdate()
    {
        Move();

    }
    #region 이동관련

    private void Move()
    {
        if (!isMove) return;
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

    #endregion

    private IEnumerator Attack()
    {
        if (guns[cureentGun] == null)
        {
            yield return 0 ;
        }
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

        SpriteRenderer gunSprite = null;

        if (guns != null && cureentGun < guns.Count && guns[cureentGun] != null)
        {
            gunSprite = guns[cureentGun].GetComponent<SpriteRenderer>();
        }

        while (elapsed < invincibleTime)
        {
            // 본체 스프라이트 끄기
            spriteRenderer.enabled = false;

            // 총 스프라이트가 있으면 끄기
            if (gunSprite != null)
            {
                gunSprite.enabled = false;
            }

            yield return new WaitForSeconds(blinkInterval / 2);

            // 본체 스프라이트 켜기
            spriteRenderer.enabled = true;

            // 총 스프라이트 있으면 켜기
            if (gunSprite != null)
            {
                gunSprite.enabled = true;
            }

            yield return new WaitForSeconds(blinkInterval / 2);

            elapsed += blinkInterval;
        }

        // 무적 해제
        isInvincible = false;

        // 최종적으로 반드시 켜놓기
        if (gunSprite != null)
        {
            gunSprite.enabled = true;
        }
        spriteRenderer.enabled = true;
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

    public void AnSkill(int _n)
    {
        if(_n == 0)
        {
            animator.SetTrigger("PistolSkill");
        }
    }

    private void Die()
    {
        Debug.Log("플레이어 죽음");
    }

    private void ChageSlot(int _n)
    {
        if (_n < 0 || _n >= guns.Count) return;
        if (_n == cureentGun) return;

        if (guns[cureentGun] != null)
            guns[cureentGun].gameObject.SetActive(false);

        cureentGun = _n;
        if (guns[cureentGun] != null)
        {
            curGun = guns[cureentGun].gameObject;
            curGun.SetActive(true);
            curGun.transform.SetParent(transform);
            curGun.transform.position = gunPoss[0].transform.position;
            guns[cureentGun].holding = true;
            slots[cureentGun].sprite = guns[cureentGun].spriteRenderer.sprite;
            slots[cureentGun].enabled = true;
        }
    }




    private void DropGun()
    {
        guns[cureentGun].holding = false;
        guns[cureentGun].transform.SetParent(null);
        guns.Remove(guns[cureentGun]);

    }

    private void PickGun(Gun _gun)
    {
        if (guns == null)
            guns = new List<Gun>();

        if (guns.Count == 0)
        {
            Debug.Log("총이 아예 없을 경우");
            guns.Add(_gun);
            cureentGun = 0;
        }
        else
        {
            Debug.Log("총이 있을 경우");
            if (cureentGun < guns.Count)
            {
                guns[cureentGun] = _gun; // 현재 장비한 총 교체
            }
            else
            {
                guns.Add(_gun);
                cureentGun = guns.Count - 1;
            }
        }

        guns[cureentGun].transform.SetParent(gameObject.transform);
        guns[cureentGun].transform.position = gunPoss[0].transform.position;
        guns[cureentGun].holding = true;
        slots[cureentGun].sprite = _gun.spriteRenderer.sprite;
        slots[cureentGun].enabled = true;
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.collider.CompareTag("Ground"))
        {
            isJump=true;
        }
      
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Pick"))
        {
            Debug.Log("Pick범위 안");
            if (Input.GetKeyDown(KeyCode.F))
            {
                Gun pickGun = collision.GetComponentInParent<Gun>();
                Debug.Log(pickGun);
                PickGun(pickGun);
            }
        }
    }
  
}
