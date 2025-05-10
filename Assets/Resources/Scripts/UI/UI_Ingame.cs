using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Ingame : MonoBehaviour
{
    [Header("Button")]
    public Button spawn_Button;

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


    public void ResetTimer()    //필요 시 호출
    {
        elapsedTime = 0f;
    }


}
