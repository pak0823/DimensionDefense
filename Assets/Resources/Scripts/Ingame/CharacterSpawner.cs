// CharacterSpawner.cs
using UnityEngine;

public abstract class CharacterSpawner : MonoBehaviour
{
    // ���� ���� å���� ����Ŭ������ ����
    public abstract Character Spawn(Vector3 position);
}
