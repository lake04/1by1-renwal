using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossZone : MonoBehaviour
{
    [SerializeField] Animator animator;
    private bool isFirst = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(!isFirst)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                animator.SetTrigger("Ui");
                isFirst = true;
            }

        }
    }
}
