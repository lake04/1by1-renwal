using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Gabriel : MonoBehaviour
{
    public float curHp;
    public float maxHp;
    public float damage;
    public bool isMove;

    public bool isBurn;

    [Header("Skill 1")]
    public GameObject skillEffect1;
    public GameObject skillMarkEffect1;
    public float skillCoolTime = 5f;
    private float currentSkillCoolTime;
    private bool isSkillReady = true;

    [Header("Skill 1")]
    public GameObject skillEffect2;
    public GameObject skillMarkEffect2;
    public Transform[] poss;


    private int skillToUse = 0;
    private bool isSkill = false;

    public GameObject target;
    public Slider hpBar;

    public Animator animator;
    public Rigidbody2D rigidbody2D;
    public SpriteRenderer spriteRenderer;

    public float invincibleTime = 1.5f;     // 무적 유지 시간
    public float blinkInterval = 0.6f;    // 깜빡임 간격

    private void Awake()
    {
        SetUp();
    }

    void Start()
    {
        UpdateHp();
    }


    void Update()
    {
        if(target == null)
        {
            target = Player.Instance.gameObject;

        }
        if (!isSkillReady && isSkill == false)
        {
            currentSkillCoolTime -= Time.deltaTime;
            if (currentSkillCoolTime <= 0)
            {
                isSkillReady = true;
            }
        }

        float distanceToTarget = Vector2.Distance(transform.position, target.transform.position);
        if (distanceToTarget < 20f && isSkillReady && isSkill == false)
        {
            ChooseAndUseSkill();
        }

    }

    public void TakeDamage(float damage)
    {
        curHp -= damage;
        UpdateHp();
        Debug.Log("피격 당함");

        if (curHp <= 0)
        {
            Die();
        }
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
                if (!isBurn)
                {
                    StartCoroutine(BurnEffect());
                }
                break;
            case Element.Water:
                TakeDamage(damage);
                //StartCoroutine(SlowDown());
                break;
            case Element.Electric:
                TakeDamage(damage);
                StartCoroutine(Stun());
                break;
        }
    }

    private IEnumerator BurnEffect()
    {
        isBurn = true;
        float burnDuration = 2f;
        float tickInterval = 0.5f;

        float timer = 0f;

        StartCoroutine(BurnVisualEffect(burnDuration, tickInterval));

        while (timer < burnDuration)
        {
            curHp -= 1;
            Debug.Log("화상 데미지");
            spriteRenderer.color = Color.red;
            if (curHp <= 0)
            {
                Destroy(gameObject);
                yield break;
            }

            yield return new WaitForSeconds(tickInterval);
            timer += tickInterval;
            spriteRenderer.color = Color.white;
        }

        isBurn = false;
        Debug.Log("화상 끝");
    }

    private IEnumerator BurnVisualEffect(float totalDuration, float tickDuration)
    {
        float timer = 0f;
        while (timer < totalDuration)
        {
            spriteRenderer.color = Color.red;

            yield return new WaitForSeconds(0.1f);

            spriteRenderer.color = Color.white;

            yield return new WaitForSeconds(tickDuration - 0.1f);
            timer += tickDuration;
        }

        spriteRenderer.color = Color.white;
    }

    private IEnumerator Stun()
    {
        Debug.Log("스턴");
        isMove = false;
        yield return new WaitForSeconds(0.4f);
        isMove = true;
    }

    //private IEnumerator SlowDown()
    //{
    //    Debug.Log("슬로우");
    //    moveSpeed = orignMoveSpeed * 0.5f;
    //    yield return new WaitForSeconds(0.4f);
    //    moveSpeed = orignMoveSpeed;
    //}
    private void Die()
    {
        Debug.Log("보스 죽음");
        animator.SetTrigger("Die");
    }
    public void OnDie()
    {
        Destroy(gameObject);
    }

    private void UpdateHp()
    {
        hpBar.value = curHp / maxHp;
    }

    #region 스킬
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
                case 2:
                    StartCoroutine(Skill3());
                    break;
            }

            isSkillReady = false;
            currentSkillCoolTime = skillCoolTime;

            skillToUse = Random.Range(0,3);
        }
    }
    private IEnumerator Skill1()
    {
        isSkill = true;
        Vector2 spawnPos = new Vector2(target.transform.position.x, target.transform.position.y);

        GameObject mark = Instantiate(skillMarkEffect1, new Vector3(spawnPos.x, spawnPos.y - 0.5f, 0), Quaternion.identity);

        yield return new WaitForSeconds(0.5f);

        Destroy(mark);
        GameObject skill = Instantiate(skillEffect1, new Vector3(spawnPos.x, spawnPos.y, 0), Quaternion.identity);
        isSkill = false;
    }

    private IEnumerator Skill2()
    {
        isSkill = true;
        int num = Random.Range(0, poss.Length); 

        GameObject mark = Instantiate(skillMarkEffect2, poss[num]); 
        Vector2 playerPos = target.transform.position;

        Vector2 dir = (playerPos - (Vector2)poss[num].position).normalized;

        yield return new WaitForSeconds(0.3f);

        Destroy(mark);
        GameObject skill = Instantiate(skillEffect2, poss[num].position, Quaternion.identity);

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        skill.transform.rotation = Quaternion.Euler(0, 0, angle);

        Rigidbody2D rb = skill.GetComponent<Rigidbody2D>();
        rb.velocity = dir * 15f;

        skill.GetComponent<FireBall>().Init(damage, dir);

        isSkill = false;
    }

    private IEnumerator Skill3()
    {
        isSkill = true;
        for (int i = 0; i < poss.Length; i++)
        {
            GameObject mark = Instantiate(skillMarkEffect2, poss[i]);

            yield return new WaitForSeconds(0.13f);

            Destroy(mark);

            Vector2 playerPos = target.transform.position;

            Vector2 dir = (playerPos - (Vector2)poss[i].position).normalized;

            GameObject skill = Instantiate(skillEffect2, poss[i].position, Quaternion.identity);

            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            skill.transform.rotation = Quaternion.Euler(0, 0, angle);

            Rigidbody2D rb = skill.GetComponent<Rigidbody2D>();
            rb.velocity = dir * 15f;

            skill.GetComponent<FireBall>().Init(damage, dir);
        }
        isSkill = false;
    }

    #endregion

    private void SetUp()
    {
        animator = GetComponent<Animator>();
        rigidbody2D = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        curHp = maxHp;
    }

    private IEnumerator InvincibleBlink()
    {
        float elapsed = 0f;

        while (elapsed < invincibleTime)
        {
            spriteRenderer.enabled = false;
            yield return new WaitForSeconds(blinkInterval / 2);

            spriteRenderer.enabled = true;
            yield return new WaitForSeconds(blinkInterval / 2);

            elapsed += blinkInterval;
        }

        spriteRenderer.enabled = true; 
    }
}