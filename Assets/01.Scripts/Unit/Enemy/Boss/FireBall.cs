using System.Collections;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using UnityEngine;

public class FireBall : MonoBehaviour
{
    [SerializeField] private float damage;
    [SerializeField] private GameObject effect;
    private float moveSpeed = 15f;

    private Rigidbody2D rb;

    private void OnEnable()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Init(float _damage, Vector3 dir)
    {
        damage = _damage;


        //float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        //gameObject.transform.rotation = Quaternion.Euler(0, 0, angle);

        //rb.velocity = dir * moveSpeed;


    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            rb.velocity = Vector2.zero;
            gameObject.GetComponent<SpriteRenderer>().enabled = false;
            collision.gameObject.GetComponent<Player>().TakeDamage(damage);
            Instantiate(effect, transform);
            Destroy(gameObject,0.4f);

        }
        if (collision.gameObject.CompareTag("Ground"))
        {
            rb.velocity = Vector2.zero;
            gameObject.GetComponent<SpriteRenderer>().enabled = false;
            Instantiate(effect, transform);
            Destroy(gameObject, 0.4f);
        }
    }
}
