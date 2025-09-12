using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public static Player Instance;

    [Header("Movement")]
    [SerializeField] public float moveSpeed = 5f;
    [SerializeField] private float acceleration = 50f;
    [SerializeField] private float decceleration = 40f;
    [SerializeField] private float velPower = 0.9f;
    [SerializeField] private float maxSpeed = 13f;
    public bool isMove = true;
    public bool isKonBack = false;

    private float x;

    [Header("Jump")]
    [SerializeField] private float jumpPower = 6f;       // ���� �ʱ� �ӵ�
    [SerializeField] private float fallMultiplier = 3f;  // �ϰ� �ӵ� ���� (���� �ӵ� ������)
    [SerializeField] private float riseMultiplier = 1.5f; // ��� �ӵ� ���� (���� �ʱ� �ӵ� ����)
    [SerializeField] private float jumpBufferTime = 0.1f; // ���� ���� �ð�
    private bool isJump = true;

    private float jumpBufferCounter;

    [Header("Setting")]
    [SerializeField] private float curHp;
    [SerializeField] private float maxHp;
    [SerializeField] public GameObject playerHitEettct;


    public float invincibleTime = 1.5f;     // ���� ���� �ð�
    public float blinkInterval = 0.6f;    // ������ ����

    private bool isInvincible = false;    // ���� �������� ����
    [SerializeField] private Image hitEffect;

    [Header("Gun")]
    public List<Gun> guns = new List<Gun>(2);
    public List<GameObject> gunObjects = new List<GameObject>(2);
    public Transform PistolSkillPos;
    private bool isAttack = true;
    private float time;
    public int currentGun = 0;

    public List<GameObject> gunPoss;
    public GameObject curGun;

    public Dictionary<Gun, float> gunCooldowns = new Dictionary<Gun, float>();

    private bool canPickUpGun = false;
    private Gun nearbyGun;

    [Header("Gun Slot")]
    [SerializeField] private List<Image> slots;
    [SerializeField] private List<Image> noSelectSlots;
    [SerializeField] private List<GameObject> slectSlotObs;
    [SerializeField] private List<GameObject> slotObs;
    
    [Header("Dash")]
    [SerializeField] private Ghost ghost;
    [SerializeField] private float dashPower = 5f;     // �뽬 �ӵ� ���� (moveSpeed * dashPower)
    [SerializeField] private float dashDuration = 0.3f; // �뽬 ���� �ð�

    [SerializeField]  private float dashCooldown;   
    private bool isDashing;                               // ���� �뽬 ������ ����
    [SerializeField]  private bool canDash = true;

    private Vector2 dir;

    public Rigidbody2D rb;
    public SpriteRenderer spriteRenderer;
    private Animator animator;

    [SerializeField] public GameObject canvas;

    [SerializeField] private CameraController followCamaer;

    #region ��������
    public float CurHp => curHp;
    public float MaxHp => maxHp;
    #endregion
    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(canvas);
            DontDestroyOnLoad(Instance);
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
        if (guns[currentGun] != null)
        {
            slots[currentGun].sprite = guns[currentGun].spriteRenderer.sprite;
            slots[currentGun].enabled = true;
        }
        SettingGun();
    }

    // Update is called once per frame
    void Update()
    {
      
        if(Input.GetKeyDown(KeyCode.C) & isAttack)
        {
            StartCoroutine(Attack());
        }
        if (Input.GetKeyDown(KeyCode.Z))
            jumpBufferCounter = jumpBufferTime;
        else
            jumpBufferCounter -= Time.deltaTime;

        Jump();

        if (Input.GetKeyDown(KeyCode.X))
        {
            if(guns[currentGun] ==null)
            {
                return;
            }
            guns[currentGun].Skill();
        }
      
        if (canPickUpGun && Input.GetKeyDown(KeyCode.F))
        {
            PickGun(nearbyGun);
        }


        for (int i = 0; i <=guns.Count; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha0 + i) && guns[currentGun].skilling == false)
            {
                ChageSlot(i-1);
            }
        }
        if (Input.GetKeyDown(KeyCode.LeftControl) && canDash)
        {
            StartCoroutine (Dash());
        }

        UpdateCooldowns();
    

        animator.SetFloat("speed", Mathf.Abs(rb.velocity.x));

        // isMoving �Ķ���ʹ� x�� 0�� �ƴ� ���� true�� ����
        animator.SetBool("isMoving", x != 0);
    }
    

    private void FixedUpdate()
    {
        Move();

    }
    #region �̵�����

    private void Move()
    {
        if (!isMove) return;

      x = Input.GetAxisRaw("Horizontal");

        // ĳ������ ���� �ӵ�(rb.velocity.x)�� ����Ͽ� �ִϸ��̼� ����


        // ���� ��ȯ ������ �״�� �Ӵϴ�.
        if (x != 0)
            spriteRenderer.flipX = x < 0;

        // ... (���� �ڵ�� ����)
        if (guns[currentGun].gunKind == GunKind.pistol)
        {
            spriteRenderer.flipX = Player.Instance.spriteRenderer.flipX;
            guns[currentGun].spriteRenderer.flipX = spriteRenderer.flipX ? false : true;
            guns[currentGun].transform.position = spriteRenderer.flipX
                ? gunPoss[1].transform.position
                : gunPoss[0].transform.position;
            animator.SetBool("isPistol", true);
            animator.SetBool("isRife", false);
        }
        if (guns[currentGun].gunKind == GunKind.rilfe)
        {
            spriteRenderer.flipX = Player.Instance.spriteRenderer.flipX;
            guns[currentGun].spriteRenderer.flipX = spriteRenderer.flipX ? false : true;
            guns[currentGun].transform.position = spriteRenderer.flipX
                ? gunPoss[2].transform.position
                : gunPoss[3].transform.position; 
                animator.SetBool("isRife", true);
             animator.SetBool("isPistol", false);
        }

        float targetSpeed = x * moveSpeed;
        float speedDiff = targetSpeed - rb.velocity.x;

        float accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? acceleration : decceleration;

        float newVelocityX = Mathf.Lerp(rb.velocity.x, targetSpeed, accelRate * Time.fixedDeltaTime);
        rb.velocity = new Vector2(newVelocityX, rb.velocity.y);

        if (Mathf.Abs(rb.velocity.x) > maxSpeed)
        {
            rb.velocity = new Vector2(Mathf.Sign(rb.velocity.x) * maxSpeed, rb.velocity.y);
            dir = new Vector2(Mathf.Sign(rb.velocity.x) * maxSpeed, rb.velocity.y);
        }
     
    }


    private void Jump()
    {
        if (jumpBufferCounter > 0f && isJump)
        {
            isJump = false;
            rb.velocity = new Vector2(rb.velocity.x, jumpPower);
            jumpBufferCounter = 0f;
        }

        // ��� �ӵ� ����
        else if (rb.velocity.y > 0)
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (riseMultiplier - 1) * Time.deltaTime;
        }
        // �ϰ� �ӵ� ����
        else if (rb.velocity.y < 0)
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }

    }


    private IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;
        this.ghost.makeGhost = true;
        isMove = false;

        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;

        // ���� ó��
        Vector2 dir = spriteRenderer.flipX ?  Vector2.left : Vector2.right;
        //rb.velocity = new Vector2(transform.localScale.x * dashPower, 0f);
        rb.velocity = new Vector2(dir.x * dashPower, 0f);

        // �뽬 ����
        yield return new WaitForSeconds(dashDuration);

        rb.gravityScale = originalGravity;
        isDashing = false;
        this.ghost.makeGhost = false;
        isMove = true;
        // ��ٿ�
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }



    #endregion

    private IEnumerator Attack()
    {
        if (guns[currentGun] == null)
        {
            yield return 0 ;
        }
        float time = guns[currentGun].attackSpeed;
        isAttack = false;
        guns[currentGun].Fire();
        CameraController.Instance.Shake(0.05f, 0.05f);
        yield return new WaitForSeconds(time);
        isAttack = true;

    }

    public void StartCooldown(Gun gun)
    {
        if (gun != null)
        {
            gunCooldowns[gun] = gun.cooldownTime;
        }
    }

    private void UpdateCooldowns()
    {
        List<Gun> gunsToUpdate = new List<Gun>(gunCooldowns.Keys);
        foreach (var gun in gunsToUpdate)
        {
            if (gunCooldowns.ContainsKey(gun))
            {
                gun.isSkill = false;
                gunCooldowns[gun] -= Time.deltaTime;

                if (gunCooldowns[gun] <= 0)
                {
                    gun.isSkill = true;

                    gunCooldowns.Remove(gun);
                }
            }
        }
    }

    public void TakeDamage(float damage)
    {
        if(isInvincible == true)
        {
            return;
        }
        curHp -= damage;
        GameObject effect = Instantiate(playerHitEettct,gameObject.transform);
        StartCoroutine(HitEffect());
        StartCoroutine(InvincibleBlink());
        Debug.Log("�ǰ� ����");

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

        if (guns != null && currentGun < guns.Count && guns[currentGun] != null)
        {
            gunSprite = guns[currentGun].GetComponent<SpriteRenderer>();
        }

        while (elapsed < invincibleTime)
        {
            // ��ü ��������Ʈ ����
            spriteRenderer.enabled = false;

            // �� ��������Ʈ�� ������ ����
            if (gunSprite != null)
            {
                gunSprite.enabled = false;
            }

            yield return new WaitForSeconds(blinkInterval / 2);

            // ��ü ��������Ʈ �ѱ�
            spriteRenderer.enabled = true;

            // �� ��������Ʈ ������ �ѱ�
            if (gunSprite != null)
            {
                gunSprite.enabled = true;
            }

            yield return new WaitForSeconds(blinkInterval / 2);

            elapsed += blinkInterval;
        }

        // ���� ����
        isInvincible = false;

        // ���������� �ݵ�� �ѳ���
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
        CircleEffect.Instance.LoadScene("Title");
        Debug.Log("�÷��̾� ����");
    }

    private void ChageSlot(int _n)
    {
        if (_n < 0 || _n >= guns.Count) return;
        if (_n == currentGun) return;

        if (guns[currentGun] != null)
        {
            gunObjects[currentGun].SetActive(false);

        }
        if (guns[currentGun] != null)
        {
            guns[currentGun].gameObject.SetActive(false);
            slectSlotObs[currentGun].gameObject.SetActive(false);
            slotObs[currentGun].gameObject.SetActive(true);
            noSelectSlots[currentGun].sprite = guns[currentGun].spriteRenderer.sprite;
        }

        currentGun = _n;
        if (guns[currentGun] != null)
        {
            Gun gun = guns[currentGun];

            gunObjects[currentGun].SetActive(true);

            if (gun.PistolSkillPos == null)
            {
                gun.PistolSkillPos = PistolSkillPos;
            }
            if (gun.gunKind == GunKind.pistol)
            {
                RectTransform rect = (RectTransform)slots[currentGun].transform;
                rect.sizeDelta = new Vector2(80f, 80f);
            }
            else if (gun.gunKind == GunKind.rilfe)
            {
                RectTransform rect = (RectTransform)slots[currentGun].transform;
                rect.sizeDelta = new Vector2(200f, 200f);
            }
            Collider2D col = gun.GetComponent<Collider2D>();
            if (col != null)
            {
                col.enabled = false;
                col.isTrigger = true;
            }

            curGun = gun.gameObject;
            curGun.SetActive(true);
            curGun.transform.SetParent(gameObject.transform);
            curGun.transform.position = gunPoss[0].transform.position;

            gun.holding = true;
        
            slots[currentGun].sprite = gun.spriteRenderer.sprite;
            slots[currentGun].enabled = true;
            slectSlotObs[currentGun].gameObject.SetActive(true);
            slotObs[currentGun].gameObject.SetActive(false);
           
        }
    }


    private void DropGun()
    {
        if (guns[currentGun] == null) return;

        Gun droppedGun = guns[currentGun];
        GameObject droppedObject = gunObjects[currentGun];

        droppedGun.holding = false;

        // ���� UI�� ��Ȱ��ȭ�մϴ�.
        slots[currentGun].enabled = false;
        slectSlotObs[currentGun].SetActive(false);
        slotObs[currentGun].SetActive(true);

        // �θ� null�� �����Ͽ� ���� �÷��̾�Լ� �и��մϴ�.
        droppedObject.transform.SetParent(null);
        droppedObject.transform.position = transform.position + new Vector3(
            1f * (spriteRenderer.flipX ? -1 : 1), 0.5f, 0);

        // ���� ����� �ٽ� Ȱ��ȭ�մϴ�.
        Rigidbody2D rb = droppedObject.GetComponent<Rigidbody2D>();
        if (rb == null) rb = droppedObject.gameObject.AddComponent<Rigidbody2D>();
        rb.isKinematic = false;
        rb.gravityScale = 2f;
        rb.AddForce(new Vector2(spriteRenderer.flipX ? -2f : 2f, 3f), ForceMode2D.Impulse);

        // �ݶ��̴��� �ٽ� Ȱ��ȭ�մϴ�.
        Collider2D col = droppedObject.GetComponent<Collider2D>();
        if (col != null)
        {
            col.enabled = true;
            col.isTrigger = false;
        }

        // �÷��̾� �κ��丮���� ���� �����մϴ�.
        guns[currentGun] = null;
        gunObjects[currentGun] = null;
        curGun = null;
       
    }




    private void PickGun(Gun _gun)
    {
        // ������ �� á�� ���� ���� ���� ����
        if (guns[0] != null && guns[1] != null)
        {
            DropGun();
        }

        // �� ���� ã��
        int emptySlot = guns[0] == null ? 0 : guns[1] == null ? 1 : currentGun;
        guns[emptySlot] = _gun;
        currentGun = emptySlot;
        curGun = _gun.gameObject;
        _gun.holding = true;

        // Rigidbody ����
        Rigidbody2D rb = _gun.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.isKinematic = true;  // ���� ���� X
        }

        // Collider ���ֱ�
        Collider2D col = _gun.GetComponent<Collider2D>();
        if (col != null) col.enabled = false;
        // ��ġ/�θ� ����
        _gun.transform.SetParent(gunPoss[0].transform);
        _gun.transform.localPosition = Vector3.zero;
        _gun.transform.localRotation = Quaternion.identity;

        slots[currentGun].sprite = _gun.spriteRenderer.sprite;
        slots[currentGun].enabled = true;
        slectSlotObs[currentGun].SetActive(true);
        slotObs[currentGun].SetActive(false);

        if (emptySlot == -1)
        {
            DropGun();
            emptySlot = currentGun;
        }

        // �� ���� ���Կ� �Ҵ��մϴ�.
        guns[emptySlot] = _gun;
        currentGun = emptySlot;

        // GameObject�� �����ͼ� ���������� ����ϴ�.
        GameObject gunObject = _gun.gameObject;
        gunObjects[emptySlot] = gunObject; // GameObject ������ �����մϴ�.

        // ���� �θ� �÷��̾�� �����ϰ� ���� ���� ������Ʈ�� ��Ȱ��ȭ�մϴ�.
        gunObject.transform.SetParent(gunPoss[0].transform);
        gunObject.transform.localPosition = Vector3.zero;
        gunObject.transform.localRotation = Quaternion.identity;
        _gun.holding = true;

        // **�� �κ��� �ٽ��Դϴ�.** �ֿ� �� ������Ʈ�� ���������� �����մϴ�.
        DontDestroyOnLoad(gunObject);

    }

    private void SettingGun()
    {
        foreach(var  gun in gunObjects)
        {
            DontDestroyOnLoad(gun);
        }
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
            canPickUpGun = true;
            Gun pickGun = collision.GetComponentInParent<Gun>();
            nearbyGun = pickGun;
            
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Pick"))
        {
            canPickUpGun = false;
            nearbyGun = null;
            Debug.Log("Pick ���� ��");
        }
    }

}
