using UnityEngine;

[CreateAssetMenu(menuName = "Factory/EasyModeFactory")]
public class EasyModeFactory : ScriptableObject, ICharacterFactory
{
    public Player easyPlayerPrefab;
    public Enemy easyEnemyPrefab;

    public Player CreatePlayer(Vector3 pos)
    {
        var p = Instantiate(easyPlayerPrefab, pos, Quaternion.identity);
        p.Initialize();
        return p;
    }

    public Enemy CreateEnemy(Vector3 pos)
    {
        var e = Instantiate(easyEnemyPrefab, pos, Quaternion.identity);
        e.Initialize();
        return e;
    }
}

[CreateAssetMenu(menuName = "Factory/HardModeFactory")]
public class HardModeFactory : ScriptableObject, ICharacterFactory
{
    public Player hardPlayerPrefab;
    public Enemy hardEnemyPrefab;

    public Player CreatePlayer(Vector3 pos)
    {
        var p = Instantiate(hardPlayerPrefab, pos, Quaternion.identity);
        p.Initialize();
        return p;
    }

    public Enemy CreateEnemy(Vector3 pos)
    {
        var e = Instantiate(hardEnemyPrefab, pos, Quaternion.identity);
        e.Initialize();
        return e;
    }
}
