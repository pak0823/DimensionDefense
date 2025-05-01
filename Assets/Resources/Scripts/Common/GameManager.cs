using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Factory")]
    public ScriptableObject factoryAsset; // EasyModeFactory �Ǵ� HardModeFactory

    ICharacterFactory factory;

    void Awake()
    {
        factory = factoryAsset as ICharacterFactory;
        if (factory == null) Debug.LogError("���丮 ������ ICharacterFactory�� �����ؾ���.");
    }

    void Start()
    {
        // ���丮�� �÷��̾�� �� ����
        factory.CreatePlayer(new Vector3(0, 0, 0));
        for (int i = 0; i < 5; i++)
            factory.CreateEnemy(new Vector3(2 * i, 0, 0));
    }
}
