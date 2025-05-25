using UnityEngine;
using UnityEngine.UI;

public class UI_Difficulty : MonoBehaviour
{
    [Header("Buttons")]
    public Button normalBtn;
    public Button hardBtn;
    public Button hellBtn;

    [Header("Button Highlight Colors")]
    public Color selectedColor = Color.yellow;
    public Color unselectedColor = Color.white;

    private void Awake()
    {
        // 버튼 콜백 등록
        normalBtn.onClick.AddListener(() => OnSelect(Difficulty.Normal));
        hardBtn.onClick.AddListener(() => OnSelect(Difficulty.Hard));
        hellBtn.onClick.AddListener(() => OnSelect(Difficulty.Hell));

        // 초기 하이라이트
        //UpdateHighlight(Shared.GameManager.CurrentDifficulty);
    }

    private void OnSelect(Difficulty diff)
    {
        // 1) GameManager에 난이도 설정
        Shared.GameManager.SetDifficulty(diff);

        // 2) UI 하이라이트 갱신
        UpdateHighlight(diff);

        Shared.SoundManager.PlaySound("TitleBtn_SFX");
        Shared.SceneFlowManager.ChangeScene("IngameScene");
    }

    private void UpdateHighlight(Difficulty selected)
    {
        // 각 버튼의 색상을 토글
        normalBtn.image.color = (selected == Difficulty.Normal) ? selectedColor : unselectedColor;
        hardBtn.image.color = (selected == Difficulty.Hard) ? selectedColor : unselectedColor;
        hellBtn.image.color = (selected == Difficulty.Hell) ? selectedColor : unselectedColor;
    }
}
