using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BatEnemy : BaseEnemy
{
    [SerializeField] private  Animator animator;
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

    void Update()
    {
        
    }

    public override void Move()
    {
        if (isMove == false || isAttacking == true)
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
                    spriteRenderer.flipX = false;
                    animator.SetInteger("state", 0);
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
                    spriteRenderer.flipX = true;
                    animator.SetInteger("state", 0);
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
    }

    public override void Init()
    {
        base.Init();
        animator = GetComponent<Animator>();
    }

    public override IEnumerator Attack()
    {
        isAttack = false;
        isAttacking = true;
        isMove = false;
        Debug.Log("공격!");
        animator.SetInteger("state", 2);
        yield return new WaitForSeconds(2f);
        isAttack = true;
        isAttacking = false;
        isMove = true;

    }
    public void OnAttack(int _n)
    {
        if (_n == 0)
        {
            attackObject.SetActive(true);
            Debug.Log("공격!!!!!!!");
            if (spriteRenderer.flipX)
                attackObject.transform.position = leftAttackObjectPos.position;
            else
                attackObject.transform.position = rigthAttackObjectPos.position;

        }
        else
        {
            attackObject.SetActive(false);
            animator.SetInteger("state", 0);
            isMove = true;
        }
    }

}
