using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;



public class GameManager : MonoBehaviour
{
    [Header("Cost Settings")]
    public int StartCost = 0;
    public int currentCost { get; private set; }
    public int maxCost = 500;
    public int gainPerSec = 1;
    private int slaveCost = 2;

    public Difficulty CurrentDifficulty { get; private set; } = Difficulty.Normal;

    public GameState State { get; private set; } = GameState.Playing;
    public event Action OnGameOver;
    public event Action OnGameRestart;

    // ���̵��� ���ȡ����� �ӵ� ���� ����
    public float[] DifficultyStatMultiplier = { 1f, 1.2f, 1.5f };    // Normal, Hard, Hell
    public float[] DifficultySpawnRate = { 1f, 0.8f, 0.6f };  // 1�ʴ� ���� �ӵ� ����, �������� ������


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

    public void SetDifficulty(Difficulty diff)  //���̵� ���� ����
    {
        CurrentDifficulty = diff;
    }
    public void ResetGameState()
    {
        currentCost = StartCost;
        Shared.UI_Ingame.UpdateCostUI();
    }

    public void GameOver()  // ���� ����
    {
        if (State == GameState.GameOver) return;
        State = GameState.GameOver;

        // �ð� ������ UI_Result.cs���� ����
        OnGameOver?.Invoke();
    }

    
    public void RestartGame() // ���� �ٽ� ����
    {
        if (State != GameState.GameOver) return;
        State = GameState.Playing;

        // �ð� �簳
        Time.timeScale = 1f;
        OnGameRestart?.Invoke();

        //���� ���� �ʱ�ȭ
        ResetGameState();

        // �� ���ε� (���� ��)
        var sceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(sceneName);
    }

    private IEnumerator CostGainRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.7f);  //������ �ʸ��� cost 1 ����
            AddCost(gainPerSec);
        }
    }

    public void EnemySlaveCostGain()
    {
        Debug.Log($"currentCost:{currentCost} -> afterCost: {currentCost + slaveCost}");
        currentCost += slaveCost;
    }

    public void SetGameSpeed(float _speed)
    {
        Time.timeScale = _speed;
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

    // ��� ��� �õ�. �����ϸ� true, �����ϸ� false ��ȯ.
    public bool TrySpendCost(int amount)
    {
        if (currentCost < amount) return false;
        currentCost -= amount;
        Shared.UI_Ingame.UpdateCostUI();
        return true;
    }
}
