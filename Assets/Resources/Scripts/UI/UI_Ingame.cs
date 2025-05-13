using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Ingame : MonoBehaviour
{
    [Header("Buttom")]
    [Tooltip("생성 트리거용 버튼")]
    public Button Random_SpawnButton;
    public Button Special_SpawnButton;



    public Button enemy_SpawnButton;    //임시 버튼

    [Header("Text")]
    public Text timer_Text;

    private float elapsedTime; //누적된 게임 시간(초)

    void Start()
    {
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

    public void OnBtnRandomSpawn()
    {
        Shared.SpawnManager.PlayerRandomSpawn();
    }

    public void OnBtnEnemySpawn()
    {
        Shared.SpawnManager.EnemyRandomSpawn();
    }


    public void ResetTimer()    //필요 시 호출
    {
        elapsedTime = 0f;
    }


}
