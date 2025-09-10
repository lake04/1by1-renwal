using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AutoReturnEffect : MonoBehaviour
{
    public float lifeTime = 1.5f;

    private void OnEnable()
    {
        CancelInvoke();
        Invoke(nameof(ReturnToPool), lifeTime);
    }

    void ReturnToPool()
    {
        Destroy(gameObject);
    }
}

