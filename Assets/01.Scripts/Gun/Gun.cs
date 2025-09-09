using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Element
{
    None = 0,
    Fire = 1,
    Water = 2,
    Electric = 3
}

public class Gun : MonoBehaviour
{
    public Element element = Element.None;

    public float damage;
    public int per; //관통력
    public float attackSpeed;

    public int ammo; //총 발사시 나오는 총알 개수

    public Transform bulletPos;
    public FireEffct fireEffect;


    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Fire()
    {
        fireEffect.FireAnim();
        GameObject bullet = Instantiate(BulletManager.instance.GetBullet((int)element), bulletPos);

        SpriteRenderer spriteRenderer = Player.Instance.spriteRenderer;

        Vector2 dir = spriteRenderer.flipX ? Vector2.left : Vector2.right;

        bullet.GetComponent<Bullet>().Init(damage,per, dir);
        //Debug.Log("공격중");
    }
}
