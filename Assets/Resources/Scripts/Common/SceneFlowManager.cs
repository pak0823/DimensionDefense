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

            // 초기 화면에서는 화면을 가린 상태로 시작
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

    // 씬이 로드될 때마다 이 메서드 호출
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 기존 씬 BGM 정지
        Shared.SoundManager.StopAllInCategory(SoundCategory.BGM);

        // 씬별 초기화
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

    // 씬 전환 요청 메서드
    public void ChangeScene(string _sceneName)
    {
        Time.timeScale = 1;
        // 페이드 아웃 애니메이션 실행 후 로드
        StartCoroutine(DoSceneChange(_sceneName, 2f));
    }

    private IEnumerator DoSceneChange(string _sceneName, float _fadeDuration)
    {
        StartCoroutine(FadeOut(_fadeDuration));
        yield return new WaitForSeconds(_fadeDuration);
        SceneManager.LoadScene(_sceneName);
    }
}
