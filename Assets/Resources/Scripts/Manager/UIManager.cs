using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public abstract class UIManager : MonoBehaviour
{
    [Header("Option Window")]
    public GameObject optionWindow;

    //옵션창 열기
    protected void SetupOptionOpen(Button openBtn, UnityAction action)
    {
        if (openBtn == null || optionWindow == null) return;
        openBtn.onClick.RemoveAllListeners();
        openBtn.onClick.AddListener(() =>
        {
            ShowPanel(optionWindow, pauseTime: true);
            action?.Invoke();
        });
    }

    //옵션창 닫기
    protected void SetupOptionClose(Button closeBtn, UnityAction action)
    {
        if (closeBtn == null || optionWindow == null) return;
        closeBtn.onClick.RemoveAllListeners();
        closeBtn.onClick.AddListener(() =>
        {
            HidePanel(optionWindow, resumeTime: true);
            action?.Invoke();
        });
    }

    protected void SetupButton(Button btn, UnityAction action)
    {
        if (btn == null) return;
        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(action);
    }

    protected void TogglePanel(GameObject panel, bool pauseTime = false)
    {
        if (panel == null) return;
        bool show = !panel.activeSelf;
        panel.SetActive(show);
        if (pauseTime)
            Time.timeScale = show ? 0f : 1f;
    }

    protected void ShowPanel(GameObject panel, bool pauseTime = false)
    {
        if (panel == null) return;
        panel.SetActive(true);
    }
    protected void HidePanel(GameObject panel, bool resumeTime = false)
    {
        if (panel == null) return;
        panel.SetActive(false);
    }
    protected void OnBtnExit(Button ExitBtn)
    {
        if (ExitBtn == null) return;
        ExitBtn.onClick.RemoveAllListeners();
        ExitBtn.onClick.AddListener(() =>
        {
            Application.Quit();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        });
    }
}
