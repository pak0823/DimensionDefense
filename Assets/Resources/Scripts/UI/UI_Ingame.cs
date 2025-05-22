using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class UI_Ingame : MonoBehaviour
{
    [Header("Buttom")]
    public Button RandomSpawn_Button;
    public Button SpecialSpawn_Button;
    public Button Menu_Button;
    public Button Save_Button;

    [Header("Panel")]
    public GameObject Menu_Panel;
    public GameObject Option_Window;

    bool isShowWindow = false;
    bool isShowPanel = false;

    [Header("Text")]
    public Text timer_Text;
    public Text cost_Text;

    private float elapsedTime; //누적된 게임 시간(초)

    public Button Test_Button;

    void Start()
    {
        Shared.UI_Ingame = this;

        elapsedTime = 0f;
        if (timer_Text == null)
            timer_Text = GetComponent<Text>();
    }

    private void Update()
    {
        // 1) 시간 누적
        elapsedTime += Time.deltaTime;

        // 2) 분·초로 포맷 (MM:SS)
        int minutes = (int)(elapsedTime / 60f);
        int seconds = (int)(elapsedTime % 60f);

        // 3) UI에 출력
        timer_Text.text = $"{minutes:00}:{seconds:00}";
    }

    public void UpdateCostUI()
    {
        if (cost_Text != null)
            cost_Text.text = $"{Shared.GameManager.currentCost}";
    }

    public void OnTestBtn()
    {
        Shared.SpawnManager.TestSpawn();
    }

    bool IsShowPanel()
    {
        if (isShowPanel)
        {
            isShowPanel = false;
            Time.timeScale = 1f;
        }
        else
        {
            isShowPanel = true;
            Time.timeScale = 0f;
        }
            
        return isShowPanel;
    }
    public void ShowPanel()
    {
        Menu_Panel.SetActive(IsShowPanel());
    }

    bool IsShowWindow()
    {
        if (isShowWindow)
        {
            isShowWindow = false;
        }
        else
        {
            isShowWindow = true;
        }

        return isShowWindow;
    }
    public void ShowWindow()
    {
        Option_Window.SetActive(IsShowWindow());
    }

    public void OnBtnRandomDefaultSpawn()
    {
        Shared.SoundManager.PlaySound("SpawnBtn_SFX");
        Shared.SpawnManager.PlayerDefaultSpawn();
    }
    public void OnBtnRandomSpecialSpawn()
    {
        Shared.SoundManager.PlaySound("SpawnBtn_SFX");
        Shared.SpawnManager.PlayerSpecialSpawn();
    }

    public void OnBtnExit()
    {
        Shared.SceneFlowManager.ChangeScene("TitleScene");
    }

    public void OnBtnSaveOption()
    {
        ShowWindow();
    }
}
