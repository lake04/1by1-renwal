using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBallDestroy : AutoReturnEffect
{
    [SerializeField] private GameObject effect;

    private void OnEnable()
    {
        Invoke("ReturnToPool", lifeTime);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected override void ReturnToPool()
    {
        gameObject.GetComponent<SpriteRenderer>().enabled = false;
        Instantiate(effect,transform);
        Destroy(gameObject,0.4f);
    }

}
