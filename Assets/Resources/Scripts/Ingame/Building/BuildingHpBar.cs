using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Building ������Ʈ�� ���� �߰��ϴ� HP Bar ���� ��ũ��Ʈ
/// ���� ��ġ�� World Space Canvas ������ �ִ� Slider�� ����մϴ�.
/// </summary>
public class BuildingHPBarController : MonoBehaviour
{
    [Header("HP Bar UI")]
    [Tooltip("World-Space Canvas ������ ��ġ�� Slider")]
    public Slider hpSlider;

    [Tooltip("HP ��ġ�� ǥ���� ���Ž� Text")]
    public Text hpText;

    private Building building;
    private int maxHp;

    void Awake()
    {
        building = GetComponentInParent<Building>();
        // Building ��ũ��Ʈ���� �ʱ�ȭ�� maxHp/currentHp�� �����ɴϴ�
        maxHp = building.stats.maxHp;
        // Slider�� �ִ밪�� ����
        if (hpSlider != null)
        {
            hpSlider.maxValue = maxHp;
            hpSlider.value = building.currentHp;
        }
    }

    void Start()
    {
        // Start�� ��� Awake�� ���� �ڿ� ����˴ϴ�.
        // ���⼭ �ʱ� CurrentHp ���� ����� �о�Ϳ�.
        hpSlider.value = building.currentHp;

        // Change �̺�Ʈ ����
        building.OnHpChanged += HandleHpChanged;

        // Text �ʱ� ����
        if (hpText != null)
            hpText.text = $"{building.currentHp}/{maxHp}";

    }
    private void HandleHpChanged(int newHp)
    {
        hpSlider.value = newHp;
    }

    void OnEnable()
    {
        // Building.TakeDamage���� hpSlider.value�� �����ϵ��� �̺�Ʈ ����
        building.OnHpChanged += UpdateHPBar;
    }

    void OnDisable()
    {
        building.OnHpChanged -= UpdateHPBar;
    }

    /// <summary>
    /// Building�� ü�� ��ȭ �� ȣ��˴ϴ�.
    /// </summary>
    private void UpdateHPBar(int currentHp)
    {
        if (hpSlider != null)
            hpSlider.value = currentHp;

        hpText.text = $"{building.currentHp}/{maxHp}";
    }
}
