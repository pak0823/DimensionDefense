using UnityEngine;
using UnityEngine.UI;

public class UI_Title : UIManager
{
    [Header("Button")]
    public Button Start_Btn, Option_Btn, Save_Btn, Exit_Btn;

    private void Awake()
    {
        SetupButton(Start_Btn, OnBtnStart);
        SetupOptionOpen(Option_Btn);
        SetupOptionClose(Save_Btn);
        OnBtnExit(Exit_Btn);
    }

    void OnBtnStart()
    {
        Shared.SoundManager.PlaySound("TitleBtn_SFX");
        Shared.SceneFlowManager.ChangeScene("IngameScene");
    }
}
