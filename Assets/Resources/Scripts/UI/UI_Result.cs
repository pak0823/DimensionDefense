using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UI_Result : UIManager
{
    [Header("Panel")]
    public GameObject panel;
    [Header("Text")]
    public Text resultText, playTimeText;
    [Header("Button")]
    public Button replayButton, HomeButton;

    private void Awake()
    {
        Shared.UI_Result = this;
        panel.gameObject.SetActive(false);

        SetupButton(replayButton, () => Shared.GameManager.RestartGame());
        SetupButton(HomeButton, () => Shared.SceneFlowManager.ChangeScene("TitleScene"));
    }

    private void OnEnable()
    {
        Shared.GameManager.OnGameOver += ShowResult;
        Shared.GameManager.OnGameRestart += HideResult;
    }
    private void OnDisable()
    {
        Shared.GameManager.OnGameOver -= ShowResult;
        Shared.GameManager.OnGameRestart -= HideResult;
    }

    private void ShowResult()
    {
        panel.gameObject.SetActive(true);
        // 결과 텍스트 설정
        if (Shared.Building != null)
        {
            bool youLost = Shared.Building.isEnemy;
            resultText.text = youLost ? "Your Lose!" : "Your Win!";
            playTimeText.text = $"PlayTime: {Shared.UI_Ingame.timer_Text.text}";
        }
        // 시간 정지
        Time.timeScale = 0f;
    }

    private void HideResult()
    {
        panel.gameObject.SetActive(false);
    }
}
