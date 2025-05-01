// CharacterSpawner.cs
using UnityEngine;

public abstract class CharacterSpawner : MonoBehaviour
{
    // 실제 생성 책임을 서브클래스에 위임
    public abstract Character Spawn(Vector3 position);
}
