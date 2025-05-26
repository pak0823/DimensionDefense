using UnityEngine;
using UnityEngine.UI;

public class UI_Title : UIManager
{
    [Header("Button")]
    public Button Start_Btn, Option_Btn, Save_Btn, Exit_Btn;

    [Header("Panel")]
    public GameObject difficulty_Panel;

    private void Awake()
    {
        SetupButton(Start_Btn, () => { TogglePanel(difficulty_Panel, pauseTime: false); Shared.SoundManager.PlaySound("CommonBtn_SFX"); });
        SetupOptionOpen(Option_Btn,()=> Shared.SoundManager.PlaySound("CommonBtn_SFX"));
        SetupOptionClose(Save_Btn, () => Shared.SoundManager.PlaySound("CommonBtn_SFX"));
        OnBtnExit(Exit_Btn);
    }
}
