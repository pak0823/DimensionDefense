using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class UI_Title : MonoBehaviour
{
    [Header("Button")]
    public Button Start_Btn;
    public Button Option_Btn;
    public Button Exit_Btn;

    [Header("Panel & Window")]
    public GameObject Panel;

    bool isShow = false;

    private void Start()
    {
        Panel.SetActive(false);
    }


    bool IsShow()
    {
        if (isShow)
            isShow = false;
        else
            isShow = true;

        return isShow;
    }

    void IsShowPanel()
    {
        Panel.SetActive(IsShow());
    }


    public void OnBtnStart()
    {
        Shared.SoundManager.PlaySound("TitleBtn_SFX");
        Shared.SceneFlowManager.ChangeScene("IngameScene");
    }
    public void OnBtnOption()
    {
        IsShowPanel();
    }
    public void OnBtnExit()
    {
        // ����� �ۿ����� �� �ڵ�� ����
        Application.Quit();

        // �����Ϳ����� �÷��� ��常 ���ߵ���
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#endif
    }

}
