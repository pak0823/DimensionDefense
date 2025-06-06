using UnityEngine;
using UnityEngine.UI;

public class UI_Ingame : UIManager
{
    [Header("Button")]
    public Button RandomSpawn_Btn, SpecialSpawn_Btn, Menu_Btn, Option_Btn, Save_Btn, Play_Btn, Exit_Btn;
    [Header("Panel & Window")]
    public GameObject Menu_Panel;
    [Header("Text")]
    public Text timer_Text, cost_Text, timeScale_Text;

    int timeScale = 1;

    private void Awake()
    {
        Shared.UI_Ingame = this;

        // ��ư ����
        SetupButton(RandomSpawn_Btn, () => {
            Shared.SoundManager.PlaySound("SpawnBtn_SFX");
            Shared.SpawnManager.PlayerDefaultSpawn();
            UpdateCostUI();
        });
        SetupButton(SpecialSpawn_Btn, () => {
            Shared.SoundManager.PlaySound("SpawnBtn_SFX");
            Shared.SpawnManager.PlayerSpecialSpawn();
            UpdateCostUI();
        });
        SetupButton(Menu_Btn, () => { Shared.SoundManager.PlaySound("SpawnBtn_SFX"); TogglePanel(Menu_Panel, pauseTime: true); });
        SetupButton(Play_Btn, () => { Shared.SoundManager.PlaySound("SpawnBtn_SFX"); TogglePanel(Menu_Panel, pauseTime: true); });
        SetupButton(Exit_Btn, () => Shared.SceneFlowManager.ChangeScene("TitleScene"));
        SetupOptionOpen(Option_Btn, () => Shared.SoundManager.PlaySound("SpawnBtn_SFX"));
        SetupOptionClose(Save_Btn, () => Shared.SoundManager.PlaySound("SpawnBtn_SFX"));

        timer_Text.text = "00:00";
        UpdateCostUI();
    }

    private float elapsedTime;

    private void Update()
    {
        elapsedTime += Time.deltaTime;
        int m = (int)(elapsedTime / 60), s = (int)(elapsedTime % 60);
        timer_Text.text = $"{m:00}:{s:00}";
    }

    public void UpdateCostUI()
    {
        if (cost_Text != null)
            cost_Text.text = $"{Shared.GameManager.currentCost}";
    }

    public Button TEST;
    public void TestSpawn()
    {
        Shared.SpawnManager.TestSpawn();
    }

    
    public void SetTimeScale()
    {
        if (timeScale > 1)
            timeScale = 0;

        Shared.GameManager.SetGameSpeed(++timeScale);
        timeScale_Text.text = $"X{timeScale}";
    }
}
