using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GradePopup : MonoBehaviour
{
    [SerializeField] private Text gradeText;
    [SerializeField] private Text waringText;
    [SerializeField] private float duration = 2f;
    [SerializeField] private float floatUpSpeed = 30f;

    private CanvasGroup canvasGroup;

    private void Awake()
    {
        canvasGroup = gameObject.GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0f;
    }

    public void ShowRatingText(Rating rating)
    {
        // 1) ��� ���ڿ�
        string gradeStr = rating.ToString();

        // 2) ��޺� ���� ���� (���ϴ� �� hex�� ����)
        string colorHex = rating switch
        {
            Rating.Normal => "#FFFFFF", // ���
            Rating.Rare => "#0070FF", // �Ķ�
            Rating.Unique => "#A335EE", // ����
            Rating.Legendary => "#FFD700", // �ݻ�
            _ => "#FFFFFF"
        };

        // 3) ��ġ �ؽ�Ʈ ����
        string formatted = $"<color={colorHex}><b>{gradeStr}</b></color> ����� ��ȯ�ƽ��ϴ�!";
        gradeText.text = formatted;


        // 4) �ִϸ��̼� ����
        StopAllCoroutines();
        StartCoroutine(DoShow());

    }

    public void ShowWarningText(int _warningcode)
    {
        waringText.color = Color.red;

        switch (_warningcode)
        {
            case 0:
                waringText.text = "���� �����մϴ�!";
                break;
            case 1:
                waringText.text = "��Ÿ���Դϴ�!";
                break;
        }

        StopAllCoroutines();
        StartCoroutine(DoShow());
    }

    private IEnumerator DoShow()
    {
        // �ʱ� ��ġ������
        canvasGroup.alpha = 1f;
        Vector3 startPos = transform.position;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            // ���� ��¦ ��������
            transform.position = startPos + Vector3.up * floatUpSpeed * (elapsed / duration);
            // ������ ����������
            canvasGroup.alpha = 1f - (elapsed / duration);
            yield return null;
        }

        // ������ Ǯ�� ��ȯ �Ǵ� �ı�
        Destroy(gameObject);
    }
}
