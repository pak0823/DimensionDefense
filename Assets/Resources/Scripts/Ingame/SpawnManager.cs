using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpawnManager : MonoBehaviour
{
    [Header("UI")]
    public Button spawnButton;

    [Header("Pool Spawners")]
    public List<CharacterSpawner> spawners;  // �ν����Ϳ� EnemyPoolSpawner, PlayerPoolSpawner �� �߰�
    public Transform spawnPointPlayer;

    void Start()
    {
        spawnButton.onClick.AddListener(SpawnRandomCharacter);
    }

    void SpawnRandomCharacter()
    {
        if (spawners == null || spawners.Count == 0) return;

        // �������� ���г� ����
        int idx = Random.Range(0, spawners.Count);
        CharacterSpawner spawner = spawners[idx];

        // Ǯ���� ������
        Character character = spawner.Spawn(spawnPointPlayer.position);
        character.Initialize();
    }
}
