using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackOb : MonoBehaviour
{
    public float damage;

    void Start()
    {

    }

    void Update()
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("�÷��̾� �ǰ�");
            collision.GetComponentInParent<Player>().TakeDamage(damage);
        }
    }
}
