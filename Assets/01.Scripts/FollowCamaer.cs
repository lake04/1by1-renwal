using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamaer : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float followSpeed;

    [Header("Camer Area")]
    [SerializeField] private float minX;
    [SerializeField] private float maxX;
    [SerializeField] private float minY;
    [SerializeField] private float maxY;

    private Camera cam;

    public bool isShaking = false;

    private void Start()
    {
        cam = Camera.main;
    }

    private void LateUpdate()
    {
        if (target != null)
        {
            if(isShaking == true)
            {
                Debug.Log("Shake¡ﬂ");
                return;
            }
            Vector3 targetPos = Vector3.Lerp(transform.position, target.position, followSpeed * Time.deltaTime);

            float camHeight = cam.orthographicSize;
            float camWidth = camHeight * cam.aspect;

            float clampedX = Mathf.Clamp(targetPos.x, minX + camWidth, maxX - camWidth);
            float clampedY = Mathf.Clamp(targetPos.y, minY + camHeight, maxY - camHeight);

            transform.position = new Vector3(clampedX, clampedY, -10);
        }
    }
}
