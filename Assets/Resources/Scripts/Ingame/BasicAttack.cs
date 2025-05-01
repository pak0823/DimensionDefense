// BasicAttack.cs
using UnityEngine;
[CreateAssetMenu(menuName = "AI/BasicAttack")]
public class BasicAttack :AttackStrategySO
{
    public override void Attack(GameObject self, GameObject target)
    {
        Debug.Log(self.name + " attacks " + target.name);
    }
}
