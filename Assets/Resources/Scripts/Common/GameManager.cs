using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState
{
    Playing,
    GameOver
}

public class GameManager : MonoBehaviour
{
    [Header("Cost Settings")]
    public int currentCost = 0;
    public int maxCost = 500;
    public int gainPerSec = 1;

    public GameState State { get; private set; } = GameState.Playing;
    public event Action OnGameOver;
    public event Action OnGameRestart;


    private void Awake()
    {
        Shared.GameManager = this;
    }

    private void Start()
    {
        StartCoroutine(CostGainRoutine());
    }

    public void GameOver()  // 게임 오버
    {
        if (State == GameState.GameOver) return;
        State = GameState.GameOver;

        //// 시간 정지
        //Time.timeScale = 0f;
        OnGameOver?.Invoke();
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
            yield return new WaitForSeconds(0.7f);  //0.7초마다 cost 1 증가
            AddCost(gainPerSec);
        }
    }

    public void AddCost(int amount)
    {
        currentCost = Mathf.Min(currentCost + amount, maxCost);
        Shared.UI_Ingame.UpdateCostUI();
        Shared.TextSetting.SetTextAlpha(currentCost);
    }

    /// <summary>
    /// 비용 사용 시도. 성공하면 true, 부족하면 false 반환.
    /// </summary>
    public bool TrySpendCost(int amount)
    {
        if (currentCost < amount) return false;
        currentCost -= amount;
        Shared.UI_Ingame.UpdateCostUI();
        return true;
    }

    


}
