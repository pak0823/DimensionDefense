using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneFlowManager : MonoBehaviour
{
    [SerializeField] private CanvasGroup fadeCanvasGroup;

    private void Awake()
    {
        if (Shared.SceneFlowManager == null)
        {
            Shared.SceneFlowManager = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;

            // �ʱ� ȭ�鿡���� ȭ���� ���� ���·� ����
            if (fadeCanvasGroup != null)
            {
                fadeCanvasGroup.alpha = 1f;
                fadeCanvasGroup.blocksRaycasts = true;
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        if (Shared.SceneFlowManager == this)
            SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // ���� �ε�� ������ �� �޼��� ȣ��
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // ���� �� BGM ����
        Shared.SoundManager.StopAllInCategory(SoundCategory.BGM);

        // ���� �ʱ�ȭ
        switch (scene.name)
        {
            case "TitleScene":
                StartCoroutine(FadeIn(1f));
                Shared.SoundManager.PlaySound("Title_BGM");
                break;
            case "IngameScene":
                StartCoroutine(FadeIn(1f));
                Shared.GameManager.ResetGameState();
                Shared.SoundManager.PlaySound("InGame_BGM");
                break;
        }
    }
    private IEnumerator FadeIn(float _duration)
    {
        if (fadeCanvasGroup == null) yield break;
        float elapsed = 0f;
        while (elapsed < _duration)
        {
            elapsed += Time.deltaTime;
            fadeCanvasGroup.alpha = 1f - Mathf.Clamp01(elapsed / _duration);
            yield return null;
        }
        fadeCanvasGroup.alpha = 0f;
        fadeCanvasGroup.blocksRaycasts = false;
    }
    private IEnumerator FadeOut(float _duration)
    {
        if (fadeCanvasGroup == null) yield break;
        fadeCanvasGroup.blocksRaycasts = true;
        float elapsed = 0f;
        while (elapsed < _duration)
        {
            elapsed += Time.deltaTime;
            fadeCanvasGroup.alpha = Mathf.Clamp01(elapsed / _duration);
            yield return null;
        }
        fadeCanvasGroup.alpha = 1f;
    }

    // �� ��ȯ ��û �޼���
    public void ChangeScene(string _sceneName)
    {
        Time.timeScale = 1;
        // ���̵� �ƿ� �ִϸ��̼� ���� �� �ε�
        StartCoroutine(DoSceneChange(_sceneName, 2f));
    }

    private IEnumerator DoSceneChange(string _sceneName, float _fadeDuration)
    {
        StartCoroutine(FadeOut(_fadeDuration));
        yield return new WaitForSeconds(_fadeDuration);
        SceneManager.LoadScene(_sceneName);
    }
}
