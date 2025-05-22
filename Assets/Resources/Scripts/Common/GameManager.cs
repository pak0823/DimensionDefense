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

    public void GameOver()  // ���� ����
    {
        if (State == GameState.GameOver) return;
        State = GameState.GameOver;

        //// �ð� ����
        //Time.timeScale = 0f;
        OnGameOver?.Invoke();
    }

    
    public void RestartGame() // ���� �ٽ� ����
    {
        if (State != GameState.GameOver) return;
        State = GameState.Playing;

        // �ð� �簳
        Time.timeScale = 1f;
        OnGameRestart?.Invoke();

        // �� ���ε� (���� ��)
        var sceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(sceneName);
    }

    private IEnumerator CostGainRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.7f);  //0.7�ʸ��� cost 1 ����
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
    /// ��� ��� �õ�. �����ϸ� true, �����ϸ� false ��ȯ.
    /// </summary>
    public bool TrySpendCost(int amount)
    {
        if (currentCost < amount) return false;
        currentCost -= amount;
        Shared.UI_Ingame.UpdateCostUI();
        return true;
    }

    


}
