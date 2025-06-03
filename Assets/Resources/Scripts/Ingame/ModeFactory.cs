using UnityEngine;

[CreateAssetMenu(menuName = "Factory/EasyModeFactory")]
public class EasyModeFactory : ScriptableObject, ICharacterFactory
{
    public Player easyPlayerPrefab;
    public Enemy easyEnemyPrefab;
    public CharacterStats easyStats;

    public Player CreatePlayer(Vector3 pos)
    {
        var p = Instantiate(easyPlayerPrefab, pos, Quaternion.identity);
        p.Initialize(easyStats);
        return p;
    }

    public Enemy CreateEnemy(Vector3 pos)
    {
        var e = Instantiate(easyEnemyPrefab, pos, Quaternion.identity);
        e.Initialize(easyStats);
        return e;
    }
}

[CreateAssetMenu(menuName = "Factory/HardModeFactory")]
public class HardModeFactory : ScriptableObject, ICharacterFactory
{
    public Player hardPlayerPrefab;
    public Enemy hardEnemyPrefab;
    public CharacterStats hardStats;   // ╫╨ех SO гр╢Г

    public Player CreatePlayer(Vector3 pos)
    {
        var p = Instantiate(hardPlayerPrefab, pos, Quaternion.identity);
        p.Initialize(hardStats);
        return p;
    }

    public Enemy CreateEnemy(Vector3 pos)
    {
        var e = Instantiate(hardEnemyPrefab, pos, Quaternion.identity);
        e.Initialize(hardStats);
        return e;
    }
}
