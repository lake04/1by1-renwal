using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    // 싱글톤 패턴으로 외부에서 쉽게 접근 가능하게 함
    public static CameraController Instance;

    [Header("카메라 추적 설정")]
    public Transform target; // 추적할 타겟 (주로 플레이어)
    public Vector3 offset;   // 타겟으로부터의 상대적 위치
    public float smoothSpeed = 0.125f; // 부드러운 움직임의 속도

    [Header("카메라 위치 제한")]
    public float minX;
    public float maxX;
    public float minY;
    public float maxY;

    [Header("카메라 흔들림")]
    private Vector3 shakeOffset = Vector3.zero; // 흔들림에 의해 추가될 오프셋

    private void Awake()
    {
        // 싱글톤 인스턴스 초기화
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

        // 1. 타겟을 따라가는 기본 위치 계산
        Vector3 desiredPosition = target.position + offset;

        // 2. 부드러운 이동을 위한 보간
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        // 3. 계산된 위치에 카메라 제한 적용
        float clampedX = Mathf.Clamp(smoothedPosition.x, minX, maxX);
        float clampedY = Mathf.Clamp(smoothedPosition.y, minY, maxY);
        Vector3 clampedPosition = new Vector3(clampedX, clampedY, smoothedPosition.z);

        // 4. 흔들림 오프셋을 최종 위치에 더함
        // 흔들림 코루틴이 동작 중일 때만 shakeOffset이 적용됨
        Vector3 finalPosition = clampedPosition + shakeOffset;

        // 5. 카메라 위치 업데이트
        transform.position = finalPosition;
    }

    // 외부에서 호출하여 카메라를 흔드는 함수
    public void Shake(float duration, float magnitude)
    {
        StartCoroutine(DoShake(duration, magnitude));
    }

    private IEnumerator DoShake(float duration, float magnitude)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            // 흔들림 오프셋을 랜덤하게 계산
            shakeOffset = new Vector3(
                Random.Range(-1f, 1f) * magnitude,
                Random.Range(-1f, 1f) * magnitude,
                0f
            );

            elapsed += Time.deltaTime;
            yield return null;
        }

        // 흔들림이 끝나면 오프셋을 0으로 되돌려 원래 위치로 복귀
        shakeOffset = Vector3.zero;
    }

    // 에디터에서 카메라 제한 영역을 시각적으로 표시
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Vector3 center = new Vector3((minX + maxX) / 2f, (minY + maxY) / 2f, transform.position.z);
        Vector3 size = new Vector3(maxX - minX, maxY - minY, 0.1f);
        Gizmos.DrawWireCube(center, size);
    }
}