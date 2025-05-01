using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraMove : MonoBehaviour
{
    [Header("Pan Speed Settings")]
    public float mousePanSpeed = 150f;      // PC용
    public float touchPanSpeed = 0.5f;   // 모바일용

    [Header("Horizontal Bounds")]
    // 카메라가 이동할 수 있는 최소/최대 X좌표
    public float minX;
    public float maxX = 42.5f;

    private void Start()
    {
        minX = transform.position.x;
    }

    void Update()
    {
        float deltaX = 0f;
        bool wantPan = false;

        // 1) 모바일 터치 드래그
        if (Application.isMobilePlatform && Input.touchCount > 0)
        {
            Touch t = Input.GetTouch(0);

            if (t.phase == TouchPhase.Moved)
            {
                deltaX = t.deltaPosition.x;
                wantPan = true;
            }
        }
        else if (Input.GetMouseButton(0))
        {
            // 마우스 X 이동량 (지난 프레임 대비)
            float mouseDeltaX = Input.GetAxis("Mouse X");

            // 드래그 방향과 반대로 이동하려면 부호를 반전
            Vector3 pos = transform.position;
            pos.x += -mouseDeltaX * mousePanSpeed * Time.deltaTime;

            // 허용 범위로 제한
            pos.x = Mathf.Clamp(pos.x, minX, maxX);

            transform.position = pos;
        }

        // 드래그 중이라면
        if (wantPan)
        {
            Vector3 pos = transform.position;
            float dpi = Screen.dpi > 0 ? Screen.dpi : 96f;        // fallback 96 DPI
            float dpiScale = dpi / 96f;                           // 기준 DPI 대비 배율

            // 드래그 방향의 반대로 이동
            pos.x += -deltaX * touchPanSpeed * dpiScale * Time.deltaTime;

            // 범위 제한
            pos.x = Mathf.Clamp(pos.x, minX, maxX);
            transform.position = pos;
        }
    }
}
