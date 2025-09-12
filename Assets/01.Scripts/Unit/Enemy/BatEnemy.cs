using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
        if (isMove == false || isAttacking == true || !isLive)
        {
            return;
        }
        Vector2 pos = new Vector2(transform.position.x, transform.position.y - 0.5f);
        float distance = Vector2.Distance(transform.position,target.position);
        if (distance <= attackDistance)
        {
            StartCoroutine(Attack());
        }
        else if (distance <= moveDistance)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                target.position,
                moveSpeed * Time.deltaTime
            );
        }

        if (target.position.x < transform.position.x)
            spriteRenderer.flipX = true;
        else
            spriteRenderer.flipX = false;

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
    public override void Die()
    {
        isLive = false;
        animator.SetTrigger("Die");
    }

    public void OnDie()
    {
        Destroy(gameObject, 0.5f);
    }


}
