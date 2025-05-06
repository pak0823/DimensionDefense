// HPBarController.cs
using UnityEngine;

public class HPBarController : MonoBehaviour
{
    private HPBar hpBarInstance;
    private Character character;
    private GameObject hpBarPrefab;
    private Transform uiCanvas;

    /// <summary>
    /// 캐릭터 재스폰/활성화할 때마다 호출합니다.
    /// 기존 HPBar를 정리한 뒤, 새로운 HPBar를 생성·초기화합니다.
    /// </summary>
    public void Initialize(Character character, GameObject hpBarPrefab, Transform uiCanvas)
    {
        // 0) 기존 HPBar 정리
        if (hpBarInstance != null)
        {
            this.character.OnHpChanged -= hpBarInstance.SetCurrentHp;
            Destroy(hpBarInstance.gameObject);
            hpBarInstance = null;
        }

        this.character = character;
        this.hpBarPrefab = hpBarPrefab;
        this.uiCanvas = uiCanvas;

        // 1) HPBar 프리팹 인스턴스화 (worldPositionStays = false)
        var go = Instantiate(hpBarPrefab, uiCanvas, false);
        hpBarInstance = go.GetComponent<HPBar>();
        if (hpBarInstance == null)
        {
            Debug.LogError("HPBarController: HPBar 컴포넌트를 찾을 수 없습니다.");
            return;
        }

        // 2) HPBar 초기화 (위치·MaxHp·CurrentHp 세팅)
        hpBarInstance.Initialize(
            character.transform,
            character.maxHp,
            character.currentHp
        );

        // 3) 체력 변화 이벤트 구독
        character.OnHpChanged += hpBarInstance.SetCurrentHp;
    }

    private void OnDisable()
    {
        // 풀링 해제 또는 비활성화 시 HPBar를 정리
        if (hpBarInstance != null)
        {
            character.OnHpChanged -= hpBarInstance.SetCurrentHp;
            Destroy(hpBarInstance.gameObject);
            hpBarInstance = null;
        }
    }
}
