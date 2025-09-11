using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    // �̱��� �������� �ܺο��� ���� ���� �����ϰ� ��
    public static CameraController Instance;

    [Header("ī�޶� ���� ����")]
    public Transform target; // ������ Ÿ�� (�ַ� �÷��̾�)
    public Vector3 offset;   // Ÿ�����κ����� ����� ��ġ
    // smoothSpeed�� 0�� ����� ���� ������ �����ϸ� �� �ε巯�����ϴ�.
    [SerializeField] private float smoothSpeed = 0.125f;
    public float baseSmoothSpeed = 0.125f; // �⺻ �ӵ�
    public float speedMultiplier = 0.05f;  // �ӵ��� ���� ����ġ

    [Header("ī�޶� ��ġ ����")]
    [Header("Camer Area")]
    [SerializeField] private float minX;
    [SerializeField] private float maxX;
    [SerializeField] private float minY;
    [SerializeField] private float maxY;

    private Camera cam;

    [Header("ī�޶� ��鸲")]
    private Vector3 shakeOffset = Vector3.zero; // ��鸲�� ���� �߰��� ������
    private Vector3 currentVelocity = Vector3.zero; // SmoothDamp�� ���� ����

    private void Awake()
    {
        // �̱��� �ν��Ͻ� �ʱ�ȭ
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        cam = Camera.main;
    }
    private void Update()
    {
        if(target ==null)
        {
            target = Player.Instance.gameObject.transform;
        }
    }

    private void FixedUpdate()
    {
        if (target != null)
        {
            // �÷��̾��� ���� �ӵ��� �����ɴϴ�.
            // Rigidbody2D.velocity�� Vector2�̹Ƿ�, x ���� ����մϴ�.
            float playerCurrentSpeed = Mathf.Abs(Player.Instance.rb.velocity.x);

            // �÷��̾��� ���� �ӵ��� ���� ī�޶� �ӵ��� �������� �����մϴ�.
            // �ӵ��� ���� �ε巯��(smoothSpeed)�� �޶����� �˴ϴ�.
            // baseSmoothSpeed: �ּ����� �ε巯�� (�÷��̾ ������ ��)
            // speedMultiplier: �ӵ��� �����Ҽ��� �ε巯���� �󸶳� �پ���� ����
            float dynamicSmoothTime = baseSmoothSpeed / (1 + playerCurrentSpeed * speedMultiplier);

            // ��ǥ ��ġ ��� (Ÿ�� ��ġ + ������)
            Vector3 desiredPosition = target.position + offset;
            Vector3 smoothedPosition = Vector3.SmoothDamp(
                transform.position,
                desiredPosition,
                ref currentVelocity,
                dynamicSmoothTime // �������� ���� �ε巯�� ���� ���
            );

            // ī�޶� ���� ����
            float camHeight = cam.orthographicSize;
            float camWidth = camHeight * cam.aspect;

            float clampedX = Mathf.Clamp(smoothedPosition.x, minX + camWidth, maxX - camWidth);
            float clampedY = Mathf.Clamp(smoothedPosition.y, minY + camHeight, maxY - camHeight);

            // ���� ��ġ ���� (���ѵ� ��ġ + ��鸲 ������)
            transform.position = new Vector3(clampedX, clampedY, transform.position.z) + shakeOffset;
        }
    }

    private void LateUpdate()
    {
     
    }

    // ���� Shake, OnDrawGizmos �޼���� �״�� ��� ����
    public void Shake(float duration, float magnitude)
    {
        StartCoroutine(DoShake(duration, magnitude));
    }

    private IEnumerator DoShake(float duration, float magnitude)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            // ��鸲 �������� �����ϰ� ���
            shakeOffset = new Vector3(
                Random.Range(-1f, 1f) * magnitude,
                Random.Range(-1f, 1f) * magnitude,
                0f
            );

            elapsed += Time.deltaTime;
            yield return null;
        }

        // ��鸲�� ������ �������� 0���� �ǵ��� ���� ��ġ�� ����
        shakeOffset = Vector3.zero;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Vector3 center = new Vector3((minX + maxX) / 2f, (minY + maxY) / 2f, transform.position.z);
        Vector3 size = new Vector3(maxX - minX, maxY - minY, 0.1f);
        Gizmos.DrawWireCube(center, size);
    }
}