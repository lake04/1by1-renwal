using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public static CameraShake instance;


    [SerializeField] private float shakeTime;
    [SerializeField] private float shakeIntensity;
    private Vector3 originRotation;
    private Vector3 originPosition;

    [SerializeField] FollowCamaer followCamaer;

    private void Awake()
    {
       if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
          
    }
    public void OnShakeCamera(float shakeTime = 1.0f, float shakeIntensity = 0.1f)
    {
        originRotation = transform.eulerAngles;
        originPosition = transform.position;

        this.shakeTime = shakeTime;
        this.shakeIntensity = shakeIntensity;

        //StopCoroutine("ShakeByRotation");
        //StartCoroutine("ShakeByRotation");

        StopCoroutine("ShakeByPosition");
        StartCoroutine("ShakeByPosition");

    }

    private IEnumerator ShakeByRotation()
    {
        Vector3 startRotation = transform.eulerAngles;

        float power = 10f;

        while (shakeTime > 0.0f)
        {
            float x = 0;
            float y = 0;
            float z = Random.Range(-1f, 1f);
            transform.rotation = Quaternion.Euler(startRotation + new Vector3(x, y, z) * shakeIntensity * power);

            shakeTime -= Time.deltaTime;
            yield return null;
        }

        transform.rotation = Quaternion.Euler(originRotation);
    }

    private IEnumerator ShakeByPosition()
    {
        float timer = 0f;
        followCamaer.isShaking = true;
        while (timer < shakeTime)
        {
            transform.localPosition = Random.insideUnitSphere * shakeIntensity + originPosition;

            timer += Time.deltaTime;
            yield return null;
        }

        followCamaer.isShaking = false;
        transform.localPosition = originPosition; // 원래 위치로 복귀
    }
}
