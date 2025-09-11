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
    // smoothSpeed를 0에 가까운 작은 값으로 설정하면 더 부드러워집니다.
    [SerializeField] private float smoothSpeed = 0.125f;
    public float baseSmoothSpeed = 0.125f; // 기본 속도
    public float speedMultiplier = 0.05f;  // 속도에 따른 가중치

    [Header("카메라 위치 제한")]
    [Header("Camer Area")]
    [SerializeField] private float minX;
    [SerializeField] private float maxX;
    [SerializeField] private float minY;
    [SerializeField] private float maxY;

    private Camera cam;

    [Header("카메라 흔들림")]
    private Vector3 shakeOffset = Vector3.zero; // 흔들림에 의해 추가될 오프셋
    private Vector3 currentVelocity = Vector3.zero; // SmoothDamp를 위한 변수

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
            // 플레이어의 실제 속도를 가져옵니다.
            // Rigidbody2D.velocity는 Vector2이므로, x 값만 사용합니다.
            float playerCurrentSpeed = Mathf.Abs(Player.Instance.rb.velocity.x);

            // 플레이어의 실제 속도에 따라 카메라 속도를 동적으로 조절합니다.
            // 속도에 따라 부드러움(smoothSpeed)이 달라지게 됩니다.
            // baseSmoothSpeed: 최소한의 부드러움 (플레이어가 멈췄을 때)
            // speedMultiplier: 속도가 증가할수록 부드러움이 얼마나 줄어들지 조절
            float dynamicSmoothTime = baseSmoothSpeed / (1 + playerCurrentSpeed * speedMultiplier);

            // 목표 위치 계산 (타겟 위치 + 오프셋)
            Vector3 desiredPosition = target.position + offset;
            Vector3 smoothedPosition = Vector3.SmoothDamp(
                transform.position,
                desiredPosition,
                ref currentVelocity,
                dynamicSmoothTime // 동적으로 계산된 부드러움 값을 사용
            );

            // 카메라 영역 제한
            float camHeight = cam.orthographicSize;
            float camWidth = camHeight * cam.aspect;

            float clampedX = Mathf.Clamp(smoothedPosition.x, minX + camWidth, maxX - camWidth);
            float clampedY = Mathf.Clamp(smoothedPosition.y, minY + camHeight, maxY - camHeight);

            // 최종 위치 설정 (제한된 위치 + 흔들림 오프셋)
            transform.position = new Vector3(clampedX, clampedY, transform.position.z) + shakeOffset;
        }
    }

    private void LateUpdate()
    {
     
    }

    // 이하 Shake, OnDrawGizmos 메서드는 그대로 사용 가능
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

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Vector3 center = new Vector3((minX + maxX) / 2f, (minY + maxY) / 2f, transform.position.z);
        Vector3 size = new Vector3(maxX - minX, maxY - minY, 0.1f);
        Gizmos.DrawWireCube(center, size);
    }
}