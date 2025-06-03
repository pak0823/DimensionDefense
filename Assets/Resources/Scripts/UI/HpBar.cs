using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// HP Bar Prefab�� �ٿ����� ��ũ��Ʈ��, ���� ��ǥ�� ��ũ�� ��ǥ�� ��ȯ�ϰ�
/// Slider ���� ��Ʈ���մϴ�.
/// </summary>
public class HPBar : MonoBehaviour
{
    [Header("UI Components")]
    public Slider slider;

    [Header("Target Settings")]
    public Transform target;
    public Vector3 offset = new Vector3(0, 2, 0);

    private RectTransform sliderRect;
    private Canvas parentCanvas;
    private Camera uiCamera;
    private int maxHp;

    public void Initialize(Transform target, int maxHp, int currentHp)
    {
        this.target = target;
        this.maxHp = maxHp;

        parentCanvas = GetComponentInParent<Canvas>();
        sliderRect = slider.GetComponent<RectTransform>();
        uiCamera = parentCanvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : parentCanvas.worldCamera;

        slider.value = (float)currentHp / maxHp;
    }

    void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }
        Vector3 screenPos = Camera.main.WorldToScreenPoint(target.position + offset);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parentCanvas.GetComponent<RectTransform>(),
            screenPos,
            uiCamera,
            out Vector2 localPoint
        );
        sliderRect.anchoredPosition = localPoint;
    }

    public void SetCurrentHp(int currentHp)
    {
        slider.value = (float)currentHp / maxHp;
    }
}