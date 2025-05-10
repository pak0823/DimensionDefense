using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Ingame : MonoBehaviour
{
    [Header("Button")]
    public Button spawn_Button;

    [Header("Text")]
    public Text timer_Text;

    private float elapsedTime; //������ ���� �ð�(��)

    void Start()
    {
        elapsedTime = 0f;
        if (timer_Text == null)
            timer_Text = GetComponent<Text>();
    }

    private void Update()
    {
        // 1) �ð� ����
        elapsedTime += Time.deltaTime;

        // 2) �С��ʷ� ���� (MM:SS)
        int minutes = (int)(elapsedTime / 60f);
        int seconds = (int)(elapsedTime % 60f);

        // 3) UI�� ���
        timer_Text.text = $"{minutes:00}:{seconds:00}";
    }


    public void ResetTimer()    //�ʿ� �� ȣ��
    {
        elapsedTime = 0f;
    }


}
