using System.Collections;
using UnityEngine;

public class BaseEnemy : MonoBehaviour
{
    public float damage;
    public float moveSpeed;
    public float orignMoveSpeed;
    public float attackDistance;
    public float moveDistance;
    public float curHp;
    public float maxHp;

    public bool isAttack;
    public bool isAttacking;
    public bool isMove;

    public bool isBurn;

    public SpriteRenderer spriteRenderer;
    public Rigidbody2D rb;
    public LayerMask targetLayer;

    public WaitForFixedUpdate wait;
    public Transform target;

    public float invincibleTime = 1.5f;     // 무적 유지 시간
    public float blinkInterval = 0.6f;    // 깜빡임 간격

    [SerializeField] protected bool isLive = false;


    void Start()
    {
        Init();
    }

    void Update()
    {
        
    }

    public virtual void Init()
    {
        curHp = maxHp;
        orignMoveSpeed = moveSpeed;
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
    }

    public void FixedUpdate()
    {
        if(!isLive)
            return;
        Move();
    }

    public virtual void Move()
    {
        if(!isMove)
        {
            return;
        }
        Vector2 pos = new Vector2(transform.position.x, transform.position.y - 0.5f);
        RaycastHit2D rightHit = Physics2D.Raycast(pos, Vector2.right, moveDistance, targetLayer);
        RaycastHit2D leftHit = Physics2D.Raycast(pos, Vector2.left, moveDistance, targetLayer);
        //Debug.DrawRay(transform.position, dir * moveDistance, Color.yellow);

        if (rightHit.collider != null)
        {
            float distance = Vector2.Distance(transform.position, rightHit.collider.transform.position);
            if(distance <= attackDistance && isAttack)
            {
                Attack();
            }

            if (rightHit.collider.CompareTag("Player"))
            {
                spriteRenderer.flipX = true;
                Vector3 targetPos = new Vector3(
            rightHit.collider.transform.position.x,
            transform.position.y, 
            transform.position.z
             );

                transform.position = Vector3.MoveTowards(
                    transform.position,
                    targetPos,
                    moveSpeed * Time.deltaTime
                );

            }

        }

        if (leftHit.collider != null)
        {
            if (leftHit.collider.CompareTag("Player"))
            {
                spriteRenderer.flipX = false;
                Vector3 targetPos = new Vector3(
            leftHit.collider.transform.position.x,
            transform.position.y,
            transform.position.z
        );

                transform.position = Vector3.MoveTowards(
                    transform.position,
                    targetPos,
                    moveSpeed * Time.deltaTime
                );

            }

        }

    }

    public virtual IEnumerator Attack()
    {
        isAttack = false;
        isAttacking = true;

        yield return new WaitForSeconds(0.3f);
    }

    public void ApplyElement(Element type, float damage)
    {
        switch (type)
        {
            case Element.None:
                TakeDamage(damage);
                break;
            case Element.Fire:
                TakeDamage(damage);
                if(!isBurn)
                {
                    StartCoroutine(BurnEffect());
                }
                break;
            case Element.Water:
                TakeDamage(damage);
                StartCoroutine(SlowDown());
                break;
            case Element.Electric:
                TakeDamage(damage);
                StartCoroutine(Stun());
                break;
        }
    }

    public void TakeDamage(float damage)
    {
        curHp-=damage;
        StartCoroutine(this.KnockBack());
        StartCoroutine(InvincibleBlink());
        Debug.Log("피격 당함");

        if (curHp<=0)
        {
            Die();
        }
    }
    private IEnumerator InvincibleBlink()
    {
        float elapsed = 0f;

        while (elapsed < invincibleTime)
        {
            spriteRenderer.enabled = false; // 스프라이트 끄기
            yield return new WaitForSeconds(blinkInterval / 2);

            spriteRenderer.enabled = true; // 스프라이트 켜기
            yield return new WaitForSeconds(blinkInterval / 2);

            elapsed += blinkInterval;
        }

        spriteRenderer.enabled = true; // 반드시 켜놓기
    }
    protected virtual IEnumerator KnockBack()
    {
        if (target != null)
        {
            Vector3 playerPos = target.transform.position;
            Vector3 dirVec = (transform.position - playerPos);
            dirVec.y = 0;
            rb.AddForce(dirVec.normalized * 3f, ForceMode2D.Impulse);
            isMove = false;
            yield return wait;
            isMove = true;
            Debug.Log("넉백");
        }

    }

    private IEnumerator BurnEffect()
    {
        isBurn = true;
        float burnDuration = 2f; 
        float tickInterval = 0.5f;

        float timer = 0f;
        while (timer < burnDuration)
        {
            curHp -= 1; 
            Debug.Log("화상 데미지");

            if (curHp <= 0)
            {
                Destroy(gameObject);
                yield break; 
            }

            yield return new WaitForSeconds(tickInterval);
            timer += tickInterval;
        }

        isBurn = false;
        Debug.Log("화상 끝");
    }

    private IEnumerator Stun()
    {
        Debug.Log("스턴");
        isMove = false;
        yield return new WaitForSeconds(0.4f);
        isMove = true;
    }

    private IEnumerator SlowDown()
    {
        Debug.Log("슬로우");
        moveSpeed = orignMoveSpeed * 0.5f;
        yield return new WaitForSeconds(0.4f);
        moveSpeed = orignMoveSpeed;
    }

    public virtual void Die()
    {
        isLive = true;
        Destroy(gameObject);
    }

}
