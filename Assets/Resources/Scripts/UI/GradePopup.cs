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
        // 1) 등급 문자열
        string gradeStr = rating.ToString();

        // 2) 등급별 색상 결정 (원하는 색 hex로 조정)
        string colorHex = rating switch
        {
            Rating.Normal => "#FFFFFF", // 흰색
            Rating.Rare => "#0070FF", // 파랑
            Rating.Unique => "#A335EE", // 보라
            Rating.Legendary => "#FFD700", // 금색
            _ => "#FFFFFF"
        };

        // 3) 리치 텍스트 조합
        string formatted = $"<color={colorHex}><b>{gradeStr}</b></color> 등급이 소환됐습니다!";
        gradeText.text = formatted;


        // 4) 애니메이션 시작
        StopAllCoroutines();
        StartCoroutine(DoShow());

    }

    public void ShowWarningText(int _warningcode)
    {
        waringText.color = Color.red;

        switch (_warningcode)
        {
            case 0:
                waringText.text = "돈이 부족합니다!";
                break;
            case 1:
                waringText.text = "쿨타임입니다!";
                break;
        }

        StopAllCoroutines();
        StartCoroutine(DoShow());
    }

    private IEnumerator DoShow()
    {
        // 초기 위치·알파
        canvasGroup.alpha = 1f;
        Vector3 startPos = transform.position;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            // 위로 살짝 떠오르고
            transform.position = startPos + Vector3.up * floatUpSpeed * (elapsed / duration);
            // 서서히 투명해지기
            canvasGroup.alpha = 1f - (elapsed / duration);
            yield return null;
        }

        // 끝나면 풀에 반환 또는 파괴
        Destroy(gameObject);
    }
}
