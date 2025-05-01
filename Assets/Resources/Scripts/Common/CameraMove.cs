using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraMove : MonoBehaviour
{
    [Header("Pan Speed Settings")]
    public float mousePanSpeed = 150f;      // PC��
    public float touchPanSpeed = 0.5f;   // ����Ͽ�

    [Header("Horizontal Bounds")]
    // ī�޶� �̵��� �� �ִ� �ּ�/�ִ� X��ǥ
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

        // 1) ����� ��ġ �巡��
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
            // ���콺 X �̵��� (���� ������ ���)
            float mouseDeltaX = Input.GetAxis("Mouse X");

            // �巡�� ����� �ݴ�� �̵��Ϸ��� ��ȣ�� ����
            Vector3 pos = transform.position;
            pos.x += -mouseDeltaX * mousePanSpeed * Time.deltaTime;

            // ��� ������ ����
            pos.x = Mathf.Clamp(pos.x, minX, maxX);

            transform.position = pos;
        }

        // �巡�� ���̶��
        if (wantPan)
        {
            Vector3 pos = transform.position;
            float dpi = Screen.dpi > 0 ? Screen.dpi : 96f;        // fallback 96 DPI
            float dpiScale = dpi / 96f;                           // ���� DPI ��� ����

            // �巡�� ������ �ݴ�� �̵�
            pos.x += -deltaX * touchPanSpeed * dpiScale * Time.deltaTime;

            // ���� ����
            pos.x = Mathf.Clamp(pos.x, minX, maxX);
            transform.position = pos;
        }
    }
}
