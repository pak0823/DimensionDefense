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
