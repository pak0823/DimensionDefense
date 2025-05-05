// SpawnManager.cs
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// CharacterDefinition ����Ʈ���� �������� ���Ǹ� ���
/// PoolManager�� ���� �ν��Ͻ��� ������ �ʱ�ȭ�մϴ�.
/// </summary>
public class SpawnManager : MonoBehaviour
{
    [Header("UI")]
    [Tooltip("���� Ʈ���ſ� ��ư")]
    public Button spawnButton;

    [Header("Character Definitions")]
    [Tooltip("Ǯ�� �� ������ ����� ���� ����Ʈ")]
    public List<CharacterDefinition> definitions;

    [Header("Spawn Settings")]
    [Tooltip("���� ��ġ Transform")]
    public Transform playerSpawnPoint;
    public Transform enemySpawnPoint;

    [Tooltip("AI ������ ���̾� ����ũ")]
    public LayerMask detectionMask;

    private void Start()
    {
        spawnButton.onClick.AddListener(SpawnRandomCharacter);
    }

    /// <summary>
    /// ������ CharacterDefinition�� ������ ���ݿ��� �ν��Ͻ��� ������ �ʱ�ȭ
    /// </summary>
    private void SpawnRandomCharacter()
    {
        if (definitions == null || definitions.Count == 0)
        {
            Debug.LogWarning("SpawnManager: definitions ����Ʈ�� ��� �ֽ��ϴ�.");
            return;
        }

        // 1) ���� ���� ����
        var def = definitions[Random.Range(0, definitions.Count)];
        Transform usePoint = (def.isEnemy ? enemySpawnPoint : playerSpawnPoint);

        // 2) Ǯ���� ������ (or Instantiate fallback)
        GameObject go = Shared.PoolManager.Spawn(def.prefab, usePoint.position);
        if (go == null)
        {
            Debug.LogError($"SpawnManager: PoolManager���� {def.prefab.name}��(��) ã�� �� �����ϴ�.");
            return;
        }

        // 3) ���� �ʱ�ȭ
        var character = go.GetComponent<Character>();
        if (character != null)
            character.Initialize(def.GetStats());

        // 4) AI ����
        var ai = go.GetComponent<AutoAI>();
        if (ai != null)
        {
            ai.detectionLayerMask = def.detectionMask;   // definition���� �ٸ� ����ũ
            ai.attackStrategy = def.attackStrategy;
        }
    }
}
