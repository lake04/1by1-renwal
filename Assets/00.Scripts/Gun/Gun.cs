using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Element
{
    None = 0,
    Fire = 1,
    Water = 2,
    Ice = 3,
    Electricity = 4
}

public class Gun : MonoBehaviour
{
    private Element element = Element.None;

    public float damage;
    public int per; //관통력
    public float attackSpeed;

    public int ammo; //총 발사시 나오는 총알 개수

    public Transform bulletPos;
    public GameObject gunEffect;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Fire()
    {
        gunEffect.SetActive(true);
        //Instantiate(BulletManager.instance.GetBullet((int)element));
        Debug.Log("공격중");
    }
}
