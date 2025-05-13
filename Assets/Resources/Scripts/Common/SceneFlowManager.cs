using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneFlowManager : MonoBehaviour
{
    private void Awake()
    {
        if (Shared.SceneFlowManager == null)
        {
            Shared.SceneFlowManager = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
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
                // ȭ�� ���̵� ��
                StartCoroutine(FadeIn(1f));
                // Ÿ��Ʋ BGM
                Shared.SoundManager.PlaySound("Title_BGM");
                break;
            case "SampleScene":
                // �ΰ��� BGM
                Shared.SoundManager.PlaySound("InGame_BGM");
                break;
        }
    }

    // ��: ���̵� �� ����
    private IEnumerator FadeIn(float _duration)
    {
        // (���⿡ ȭ�� ���̵� ����)
        yield return new WaitForSeconds(_duration);
    }

    /// <summary>
    /// �� ��ȯ ��û �޼���
    /// SceneManager.LoadScene�� ���� ȣ������ �ʰ� �� �޼��� ����
    /// </summary>
    public void ChangeScene(string _sceneName)
    {
        // ���̵� �ƿ� �ִϸ��̼� ���� �� �ε�
        StartCoroutine(DoSceneChange(_sceneName, 0.5f));
    }

    private IEnumerator DoSceneChange(string _sceneName, float _fadeDuration)
    {
        // (���⿡ ȭ�� ���̵� �ƿ� ����)
        yield return new WaitForSeconds(_fadeDuration);
        SceneManager.LoadScene(_sceneName);
    }
}
