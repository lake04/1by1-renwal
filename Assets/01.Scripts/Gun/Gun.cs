using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public enum Element
{
    None = 0,
    Fire = 1,
    Water = 2,
    Electric = 3
}

public enum GunKind
{
    pistol,
    rilfe,
    shotgun
}
     

public class Gun : MonoBehaviour
{
    public Element element = Element.None;
    public GunKind gunKind;

    public float damage;
    public int per; //관통력
    public float attackSpeed;

    public int ammo; //총 발사시 나오는 총알 개수

    public Transform bulletPos;
    public FireEffct fireEffect;

    public bool isSkill = true;
    public bool skilling;
    public float skillTime;

    public Transform PistolSkillPos;



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

  

    public void Skill()
    {
        if (!isSkill)
        {
            return;
        }
        switch (gunKind)
        {
            case GunKind.pistol:
                StartCoroutine(PistolSkill());
                break;
            case GunKind.rilfe:
                break;
            case GunKind.shotgun:
                break;

        }

    }

    private IEnumerator PistolSkill()
    {
        Debug.Log("피스톨 스킬 발동중");
        gameObject.GetComponent<SpriteRenderer>().enabled = false;
        Player.Instance.AnSkill(0);
        skilling = true;
        isSkill = false;
        Player.Instance.isMove = false;
        for (int i = 0;i<6;i++)
        {
            GameObject bullet = Instantiate(BulletManager.instance.GetBullet((int)element), Player.Instance.PistolSkillPos);

            SpriteRenderer spriteRenderer = Player.Instance.spriteRenderer;

            Vector2 dir = spriteRenderer.flipX ? Vector2.left : Vector2.right;
            
            bullet.GetComponent<Bullet>().Init(damage, per, dir);
            CameraShake.instance.OnShakeCamera(0.25f, 0.05f);
            StartCoroutine(KnockBack());
            yield return new WaitForSeconds(0.2f);
        }
        gameObject.GetComponent<SpriteRenderer>().enabled = true;
        isSkill = true;
        skilling = false;
        Player.Instance.isMove = true;
    }

    protected virtual IEnumerator KnockBack()
    {
        if(!Player.Instance.isKonBack)
        {
           yield return 0;
        }
        Player.Instance.isKonBack = true;
        Vector3 dirVec = Player.Instance.spriteRenderer.flipX == true ? Vector3.right : Vector3.left;
        dirVec.y = 0;
        Player.Instance.rb.AddForce(dirVec.normalized * 1f, ForceMode2D.Impulse);
        yield return new WaitForSeconds(0.3f);
        Debug.Log("넉백");
        Player.Instance.isKonBack = false;
    }
}
