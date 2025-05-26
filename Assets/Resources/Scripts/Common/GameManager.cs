using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;



public class GameManager : MonoBehaviour
{
    [Header("Cost Settings")]
    public int currentCost = 0;
    public int maxCost = 500;
    public int gainPerSec = 1;

    public Difficulty CurrentDifficulty { get; private set; } = Difficulty.Normal;

    public GameState State { get; private set; } = GameState.Playing;
    public event Action OnGameOver;
    public event Action OnGameRestart;

    // 난이도별 스탯·스폰 속도 보정 비율
    public float[] DifficultyStatMultiplier = { 1f, 1.2f, 1.5f };    // Normal, Hard, Hell
    public float[] DifficultySpawnRate = { 1f, 0.8f, 0.6f };  // 1초당 스폰 속도 조정, 낮을수록 빠르게


    private void Awake()
    {
        if (Shared.GameManager == null)
        {
            Shared.GameManager = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        StartCoroutine(CostGainRoutine());
    }

    public void SetDifficulty(Difficulty diff)  //난이도 설정 적용
    {
        CurrentDifficulty = diff;
        //Debug.Log($"난이도 변경: {diff}");
    }

    public void GameOver()  // 게임 오버
    {
        if (State == GameState.GameOver) return;
        State = GameState.GameOver;

        // 시간 정지는 UI_Result.cs에서 실행
        OnGameOver?.Invoke();
        Debug.Log("게임 끝");
    }

    
    public void RestartGame() // 게임 다시 시작
    {
        if (State != GameState.GameOver) return;
        State = GameState.Playing;

        // 시간 재개
        Time.timeScale = 1f;
        OnGameRestart?.Invoke();

        // 씬 리로드 (현재 씬)
        var sceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(sceneName);
    }

    private IEnumerator CostGainRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.7f);  //정해진 초마다 cost 1 증가
            AddCost(gainPerSec);
        }
    }

    public void AddCost(int amount)
    {
        if(Shared.UI_Ingame != null)
        {
            currentCost = Mathf.Min(currentCost + amount, maxCost);
            Shared.UI_Ingame.UpdateCostUI();
            Shared.TextSetting.SetTextAlpha(currentCost);
        }
    }

    // 비용 사용 시도. 성공하면 true, 부족하면 false 반환.
    public bool TrySpendCost(int amount)
    {
        if (currentCost < amount) return false;
        currentCost -= amount;
        Shared.UI_Ingame.UpdateCostUI();
        return true;
    }
}
