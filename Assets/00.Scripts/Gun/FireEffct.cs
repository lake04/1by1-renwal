using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireEffct : MonoBehaviour
{
    private Animator animator;

    //private void OnEnable()
    //{
    //    FireAnim();
    //}
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {

    }


    public void FireAnim()
    {
        Debug.Log("√— πﬂªÁ ¿Ã∆Â∆Æ");
        A_OnToggle(1);
        animator.SetTrigger("fire");
    }

    public void A_OnToggle(int _n)
    {
        if (_n == 0) gameObject.SetActive(false);
        else gameObject.SetActive(true);

    }

}
