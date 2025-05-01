using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpawnManager : MonoBehaviour
{
    [Header("UI")]
    public Button spawnButton;

    [Header("Pool Spawners")]
    public List<CharacterSpawner> spawners;  // 인스펙터에 EnemyPoolSpawner, PlayerPoolSpawner 등 추가
    public Transform spawnPointPlayer;

    void Start()
    {
        spawnButton.onClick.AddListener(SpawnRandomCharacter);
    }

    void SpawnRandomCharacter()
    {
        if (spawners == null || spawners.Count == 0) return;

        // 랜덤으로 스패너 선택
        int idx = Random.Range(0, spawners.Count);
        CharacterSpawner spawner = spawners[idx];

        // 풀에서 꺼내기
        Character character = spawner.Spawn(spawnPointPlayer.position);
        character.Initialize();
    }
}
