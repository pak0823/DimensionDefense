// IAttackStrategy.cs
using UnityEngine;

public interface IAttackStrategy
{
    void Attack(GameObject self, GameObject target);
}

// AttackStrategySO
public abstract class AttackStrategySO : ScriptableObject, IAttackStrategy
{
    public abstract void Attack(GameObject self, GameObject target);
}


