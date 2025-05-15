using UnityEngine;

public class PulsingScale : MonoBehaviour
{
    [Header("Scale Settings")]
    Vector3 minScale;               // �ּ� ũ��
    Vector3 maxScale;        // �ִ� ũ��
    float speed = 1f;                             // �ִϸ��̼� �ӵ�

    private void Start()
    {
        minScale = transform.localScale;
        maxScale = minScale * 1.5f;
    }

    void Update()
    {
        // -1 ~ +1 ������ ���̴��� ���� 0~1�� ��ȯ
        float t = (Mathf.Sin(Time.time * speed) + 1f) * 0.5f;
        // �ּ�~�ִ� ������ ���̸� ����
        transform.localScale = Vector3.Lerp(minScale, maxScale, t);
    }
}
