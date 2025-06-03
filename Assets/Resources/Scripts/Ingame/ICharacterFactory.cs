using UnityEngine;

public interface ICharacterFactory
{
    Player CreatePlayer(Vector3 position);
    Enemy CreateEnemy(Vector3 position);
}
