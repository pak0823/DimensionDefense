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

    // 씬이 로드될 때마다 이 메서드 호출
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 기존 씬 BGM 정지
        Shared.SoundManager.StopAllInCategory(SoundCategory.BGM);

        // 씬별 초기화
        switch (scene.name)
        {
            case "TitleScene":
                // 화면 페이드 인
                StartCoroutine(FadeIn(1f));
                // 타이틀 BGM
                Shared.SoundManager.PlaySound("Title_BGM");
                break;
            case "SampleScene":
                // 인게임 BGM
                Shared.SoundManager.PlaySound("InGame_BGM");
                break;
        }
    }

    // 예: 페이드 인 구현
    private IEnumerator FadeIn(float _duration)
    {
        // (여기에 화면 페이드 로직)
        yield return new WaitForSeconds(_duration);
    }

    /// <summary>
    /// 씬 전환 요청 메서드
    /// SceneManager.LoadScene를 직접 호출하지 않고 이 메서드 쓰기
    /// </summary>
    public void ChangeScene(string _sceneName)
    {
        // 페이드 아웃 애니메이션 실행 후 로드
        StartCoroutine(DoSceneChange(_sceneName, 0.5f));
    }

    private IEnumerator DoSceneChange(string _sceneName, float _fadeDuration)
    {
        // (여기에 화면 페이드 아웃 로직)
        yield return new WaitForSeconds(_fadeDuration);
        SceneManager.LoadScene(_sceneName);
    }
}
