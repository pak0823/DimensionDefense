using UnityEngine;

public class PulsingScale : MonoBehaviour
{
    [Header("Scale Settings")]
    Vector3 minScale;               // 최소 크기
    Vector3 maxScale;        // 최대 크기
    float speed = 1f;                             // 애니메이션 속도

    private void Start()
    {
        minScale = transform.localScale;
        maxScale = minScale * 1.5f;
    }

    void Update()
    {
        // -1 ~ +1 사이의 사이누스 값을 0~1로 변환
        float t = (Mathf.Sin(Time.time * speed) + 1f) * 0.5f;
        // 최소~최대 스케일 사이를 보간
        transform.localScale = Vector3.Lerp(minScale, maxScale, t);
    }
}
