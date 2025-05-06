// HPBarController.cs
using UnityEngine;

public class HPBarController : MonoBehaviour
{
    private HPBar hpBarInstance;
    private Character character;
    private GameObject hpBarPrefab;
    private Transform uiCanvas;

    /// <summary>
    /// ĳ���� �罺��/Ȱ��ȭ�� ������ ȣ���մϴ�.
    /// ���� HPBar�� ������ ��, ���ο� HPBar�� �������ʱ�ȭ�մϴ�.
    /// </summary>
    public void Initialize(Character character, GameObject hpBarPrefab, Transform uiCanvas)
    {
        // 0) ���� HPBar ����
        if (hpBarInstance != null)
        {
            this.character.OnHpChanged -= hpBarInstance.SetCurrentHp;
            Destroy(hpBarInstance.gameObject);
            hpBarInstance = null;
        }

        this.character = character;
        this.hpBarPrefab = hpBarPrefab;
        this.uiCanvas = uiCanvas;

        // 1) HPBar ������ �ν��Ͻ�ȭ (worldPositionStays = false)
        var go = Instantiate(hpBarPrefab, uiCanvas, false);
        hpBarInstance = go.GetComponent<HPBar>();
        if (hpBarInstance == null)
        {
            Debug.LogError("HPBarController: HPBar ������Ʈ�� ã�� �� �����ϴ�.");
            return;
        }

        // 2) HPBar �ʱ�ȭ (��ġ��MaxHp��CurrentHp ����)
        hpBarInstance.Initialize(
            character.transform,
            character.maxHp,
            character.currentHp
        );

        // 3) ü�� ��ȭ �̺�Ʈ ����
        character.OnHpChanged += hpBarInstance.SetCurrentHp;
    }

    private void OnDisable()
    {
        // Ǯ�� ���� �Ǵ� ��Ȱ��ȭ �� HPBar�� ����
        if (hpBarInstance != null)
        {
            character.OnHpChanged -= hpBarInstance.SetCurrentHp;
            Destroy(hpBarInstance.gameObject);
            hpBarInstance = null;
        }
    }
}
