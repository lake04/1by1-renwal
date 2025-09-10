using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Gabriel : MonoBehaviour
{
    public float curHp;
    public float maxHp;
    public float damage;

    [Header("Skill 1")]
    public GameObject skillEffect1;
    public GameObject skillMarkEffect1;
    public float skillCoolTime = 5f;
    private float currentSkillCoolTime;
    private bool isSkillReady = true;

    [Header("Skill 1")]
    public GameObject skillEffect2;


    private int skillToUse = 0;

    public GameObject target;

    public Animator animator;
    public Rigidbody2D rigidbody2D;

    private void Awake()
    {
        SetUp();
    }

    void Start()
    {

    }


    void Update()
    {
        if (!isSkillReady)
        {
            currentSkillCoolTime -= Time.deltaTime;
            if (currentSkillCoolTime <= 0)
            {
                isSkillReady = true;
            }
        }

        float distanceToTarget = Vector2.Distance(transform.position, target.transform.position);
        if (distanceToTarget < 10f && isSkillReady)
        {
            ChooseAndUseSkill();
        }
    }

    public void TakeDamage(float damage)
    {
        curHp -= damage;

        Debug.Log("피격 당함");

        if (curHp <= 0)
        {
            Die();
        }
    }
    private void Die()
    {
        Debug.Log("보스 죽음");
    }


    private void ChooseAndUseSkill()
    {
        if (isSkillReady)
        {
            switch (skillToUse)
            {
                case 0:
                    StartCoroutine(Skill1());
                    break;
                case 1:
                    StartCoroutine(Skill2());
                    break;
            }

            isSkillReady = false;
            currentSkillCoolTime = skillCoolTime;

            skillToUse = (skillToUse + 1) % 2;
        }
    }
    private IEnumerator Skill1()
    {
        Vector2 spawnPos = new Vector2(target.transform.position.x, target.transform.position.y);

        GameObject mark = Instantiate(skillMarkEffect1, new Vector3(spawnPos.x, spawnPos.y - 0.5f, 0), Quaternion.identity);

        yield return new WaitForSeconds(0.5f);

        Destroy(mark);
        GameObject skill = Instantiate(skillEffect1, new Vector3(spawnPos.x, spawnPos.y, 0), Quaternion.identity);

    }

    private IEnumerator Skill2()
    {

        yield return new WaitForSeconds(0.5f);

        GameObject skill = Instantiate(skillEffect2,transform);
        Vector2 dir = target.transform.position - transform.position;
        skill.GetComponent<FireBall>().Init(damage, dir);
    }

    private void SetUp()
    {
        animator = GetComponent<Animator>();
        rigidbody2D = GetComponent<Rigidbody2D>();
        curHp = maxHp;
    }
}