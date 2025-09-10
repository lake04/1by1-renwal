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
    public float smoothSpeed = 0.125f; // �ε巯�� �������� �ӵ�

    [Header("ī�޶� ��ġ ����")]
    public float minX;
    public float maxX;
    public float minY;
    public float maxY;

    [Header("ī�޶� ��鸲")]
    private Vector3 shakeOffset = Vector3.zero; // ��鸲�� ���� �߰��� ������

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
    }

    private void LateUpdate()
    {
        if (target == null) return;

        // 1. Ÿ���� ���󰡴� �⺻ ��ġ ���
        Vector3 desiredPosition = target.position + offset;

        // 2. �ε巯�� �̵��� ���� ����
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        // 3. ���� ��ġ�� ī�޶� ���� ����
        float clampedX = Mathf.Clamp(smoothedPosition.x, minX, maxX);
        float clampedY = Mathf.Clamp(smoothedPosition.y, minY, maxY);
        Vector3 clampedPosition = new Vector3(clampedX, clampedY, smoothedPosition.z);

        // 4. ��鸲 �������� ���� ��ġ�� ����
        // ��鸲 �ڷ�ƾ�� ���� ���� ���� shakeOffset�� �����
        Vector3 finalPosition = clampedPosition + shakeOffset;

        // 5. ī�޶� ��ġ ������Ʈ
        transform.position = finalPosition;
    }

    // �ܺο��� ȣ���Ͽ� ī�޶� ���� �Լ�
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

    // �����Ϳ��� ī�޶� ���� ������ �ð������� ǥ��
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Vector3 center = new Vector3((minX + maxX) / 2f, (minY + maxY) / 2f, transform.position.z);
        Vector3 size = new Vector3(maxX - minX, maxY - minY, 0.1f);
        Gizmos.DrawWireCube(center, size);
    }
}