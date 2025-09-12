using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghost : MonoBehaviour
{
    public float ghostDelay;
    private float ghostDelyTime;
    public GameObject ghost;
    public bool makeGhost = false;

    void Start()
    {
        this.ghostDelyTime = this.ghostDelay;
    }

    private void FixedUpdate()
    {
        if(this.makeGhost)
        {
            if(this.ghostDelyTime >0)
            {
                this.ghostDelyTime -=Time.deltaTime;
            }
            else
            {
                Debug.Log("고스트 소환");
                GameObject currentGshost = Instantiate(this.ghost,this.transform.position, this.transform.rotation);
                Sprite currentSprite = this.GetComponent<SpriteRenderer>().sprite;
                currentGshost.transform.localScale = this.transform.localScale;
                currentGshost.GetComponent<SpriteRenderer>().sprite = currentSprite;
                currentGshost.GetComponent<SpriteRenderer>().flipX = Player.Instance.spriteRenderer.flipX;
                this.ghostDelyTime = this.ghostDelay;
                Destroy(currentGshost, 1f);
            }
        }
    }
}
