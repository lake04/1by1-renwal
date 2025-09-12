using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using static UnityEngine.GraphicsBuffer;

public class MouseEnemy : BaseEnemy
{

    [SerializeField] private Animator animator;
    [SerializeField] private GameObject attackObject;
    [SerializeField] private Transform leftAttackObjectPos;
    [SerializeField] private Transform rigthAttackObjectPos;


    private void Awake()
    {
        Init();
    }

    void Start()
    {

    }

    public void FixedUpdate()
    {
        if (!isLive)
            return;
        this.Move();
    }

    public override void Init()
    {
        base.Init();
        animator = GetComponent<Animator>();
    }

    public override void Move()
    {
        if (isMove == false || isAttacking == true || !isLive)
        {
            return;
        }
        Vector2 pos = new Vector2(transform.position.x, transform.position.y - 0.5f);
        RaycastHit2D rightHit = Physics2D.Raycast(pos, Vector2.right, moveDistance, targetLayer);
        RaycastHit2D leftHit = Physics2D.Raycast(pos, Vector2.left, moveDistance, targetLayer);
        //Debug.DrawRay(transform.position, dir * moveDistance, Color.yellow);

        if (rightHit.collider != null)
        {
            if (rightHit.collider.CompareTag("Player"))
            {

                float distance = Vector2.Distance(transform.position, rightHit.collider.transform.position);
                if (distance <= attackDistance && isAttack)
                {
                    StartCoroutine(Attack());
                }
                else
                {
                    target = rightHit.collider.transform;
                    spriteRenderer.flipX = true;
                    animator.SetInteger("state", 1);
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

        }
        else if (leftHit.collider != null)
        {
            if (leftHit.collider.CompareTag("Player"))
            {
                float distance = Vector2.Distance(transform.position, leftHit.collider.transform.position);
                if (distance <= attackDistance && isAttack)
                {
                    StartCoroutine(Attack());
                }
                else
                {
                    target = leftHit.collider.transform;
                    spriteRenderer.flipX = false;
                    animator.SetInteger("state", 1);
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
        //else
        //{
        //    animator.SetInteger("state", 0);
        //}
    }


    public override IEnumerator Attack()
    {
        isAttack = false;
        isAttacking = true;
        isMove = false;
        Debug.Log("°ø°Ý!");
        animator.SetInteger("state", 2);
        yield return new WaitForSeconds(2f);
        isAttack = true;
        isAttacking = false;
        isMove = true;
    }
    protected override IEnumerator KnockBack()
    {
        animator.SetInteger("state", 0);
        if (target != null)
        {
            Vector3 playerPos = target.transform.position;
            Vector3 dirVec = (transform.position - playerPos);
            dirVec.y = 0;
            rb.AddForce(dirVec.normalized * 3f, ForceMode2D.Impulse);
            isMove = false;
            yield return wait;
            isMove = true;
            Debug.Log("³Ë¹é");
        }
       
    }

    public override void Die()
    {
        isLive = false;
        animator.SetTrigger("Die");
    }

    public void OnDie()
    {
        Destroy(gameObject,0.5f);
    }

    public void OnAttack(int _n)
    {
        if(_n == 0)
        {
            attackObject.SetActive(true);

            if (spriteRenderer.flipX)
                attackObject.transform.position = rigthAttackObjectPos.position;
            else
                attackObject.transform.position = leftAttackObjectPos.position;

        }
        else
        {
            attackObject.SetActive(false);
            animator.SetInteger("state", 0);
            isMove = true;
        }
    }
}
