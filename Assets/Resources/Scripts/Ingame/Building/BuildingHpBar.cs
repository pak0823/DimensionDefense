using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Building 오브젝트에 직접 추가하는 HP Bar 제어 스크립트
/// 씬에 배치된 World Space Canvas 하위에 있는 Slider를 사용합니다.
/// </summary>
public class BuildingHPBarController : MonoBehaviour
{
    [Header("HP Bar UI")]
    [Tooltip("World-Space Canvas 하위에 배치된 Slider")]
    public Slider hpSlider;

    [Tooltip("HP 수치를 표시할 레거시 Text")]
    public Text hpText;

    private Building building;
    private int maxHp;

    void Awake()
    {
        building = GetComponentInParent<Building>();
        // Building 스크립트에서 초기화된 maxHp/currentHp를 가져옵니다
        maxHp = building.stats.maxHp;
        // Slider의 최대값을 세팅
        if (hpSlider != null)
        {
            hpSlider.maxValue = maxHp;
            hpSlider.value = building.currentHp;
        }
    }

    void Start()
    {
        // Start는 모든 Awake가 끝난 뒤에 실행됩니다.
        // 여기서 초기 CurrentHp 값을 제대로 읽어와요.
        hpSlider.value = building.currentHp;

        // Change 이벤트 구독
        building.OnHpChanged += HandleHpChanged;

        // Text 초기 세팅
        if (hpText != null)
            hpText.text = $"{building.currentHp}/{maxHp}";

    }
    private void HandleHpChanged(int newHp)
    {
        hpSlider.value = newHp;
    }

    void OnEnable()
    {
        // Building.TakeDamage에서 hpSlider.value를 갱신하도록 이벤트 구독
        building.OnHpChanged += UpdateHPBar;
    }

    void OnDisable()
    {
        building.OnHpChanged -= UpdateHPBar;
    }

    /// <summary>
    /// Building의 체력 변화 시 호출됩니다.
    /// </summary>
    private void UpdateHPBar(int currentHp)
    {
        if (hpSlider != null)
            hpSlider.value = currentHp;

        hpText.text = $"{building.currentHp}/{maxHp}";
    }
}
