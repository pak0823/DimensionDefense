using UnityEngine;

[CreateAssetMenu(menuName = "Stats/BuildingStats")]
public class BuildingStats : ScriptableObject
{
    public int maxHp = 1000;
    // 필요하면 방어력, 회복 속도 등 필드 추가
}
