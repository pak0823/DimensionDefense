using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements.Experimental;

public class UI_Result : UIManager
{
    [Header("Panel")]
    public GameObject panel;
    [Header("Text")]
    public Text resultText, playTimeText, difficultyText;
    [Header("Button")]
    public Button replayButton, HomeButton;

    public bool destroyBuilding;

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

        string colorHex = Shared.GameManager.CurrentDifficulty switch
        {
            Difficulty.Normal => "#000000", // 검은색
            Difficulty.Hard => "#FF2A2A", // 붉은색
            Difficulty.Hell => "#4D0000", // 검붉은색
            _ => "#000000"
        };

        // 결과 텍스트 설정
        if (Shared.Building != null)
        {
            difficultyText.text = $"[<color={colorHex}>{Shared.GameManager.CurrentDifficulty.ToString()}</color>]";
            resultText.text = destroyBuilding ? "Your Win!" : "Your Lose!";
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
