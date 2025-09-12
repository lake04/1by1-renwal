using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public Element element;
    private float damage;
    private float per;
    [SerializeField] GameObject effect;

    private Vector3 startPos;
    private float maxDistanc = 15f;

    private Rigidbody2D rb;

    private void OnEnable()
    {
        if(rb == null)
        {
            rb = GetComponent<Rigidbody2D>();
        }
    }

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        float curPos = Vector2.Distance(startPos, transform.position);
        if (curPos >= maxDistanc)
        {
            Destroy();
        }
    }

    public void Init(float _damage,float _per, Vector3 dir)
    {
        damage = _damage;
        per = _per;
        rb.velocity = dir * 80;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        BaseEnemy enemy = collision.GetComponent<BaseEnemy>();
        if (enemy != null)
        {
            enemy.ApplyElement(element, damage);
            per--;
            if(per<0)
                Destroy();

        }
        if (collision.gameObject.CompareTag("Boss"))
        {
            collision.gameObject.GetComponent<Gabriel>().TakeDamage(damage);
            Destroy();
        }
    }

    private void Destroy()
    {
        Instantiate(effect, transform.position,Quaternion.identity);
        Destroy(gameObject);
    }


}
