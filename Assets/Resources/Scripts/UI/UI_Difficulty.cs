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
        // ��ư �ݹ� ���
        normalBtn.onClick.AddListener(() => OnSelect(Difficulty.Normal));
        hardBtn.onClick.AddListener(() => OnSelect(Difficulty.Hard));
        hellBtn.onClick.AddListener(() => OnSelect(Difficulty.Hell));

        // �ʱ� ���̶���Ʈ
        //UpdateHighlight(Shared.GameManager.CurrentDifficulty);
    }

    private void OnSelect(Difficulty diff)
    {
        // 1) GameManager�� ���̵� ����
        Shared.GameManager.SetDifficulty(diff);

        // 2) UI ���̶���Ʈ ����
        UpdateHighlight(diff);

        Shared.SoundManager.PlaySound("TitleBtn_SFX");
        Shared.SceneFlowManager.ChangeScene("IngameScene");
    }

    private void UpdateHighlight(Difficulty selected)
    {
        // �� ��ư�� ������ ���
        normalBtn.image.color = (selected == Difficulty.Normal) ? selectedColor : unselectedColor;
        hardBtn.image.color = (selected == Difficulty.Hard) ? selectedColor : unselectedColor;
        hellBtn.image.color = (selected == Difficulty.Hell) ? selectedColor : unselectedColor;
    }
}
