using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Cost Settings")]
    public int currentCost = 0;
    public int maxCost = 500;
    public int gainPerSec = 1;


    private void Awake()
    {
        Shared.GameManager = this;
    }

    private void Start()
    {
        StartCoroutine(CostGainRoutine());
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
