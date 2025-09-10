using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pick : MonoBehaviour
{
    [SerializeField] public GameObject keyImg;

    private void OnEnable()
    {
        keyImg.SetActive(false);
    }
    void Start()
    {
        
    }

    void Update()
    {
        Gun gun = gameObject.GetComponentInParent<Gun>();
        if(gun.holding == true)
        {
            keyImg.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            keyImg.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            keyImg.SetActive(false);
        }
    }
}
