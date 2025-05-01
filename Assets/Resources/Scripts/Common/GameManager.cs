using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Factory")]
    public ScriptableObject factoryAsset; // EasyModeFactory 또는 HardModeFactory

    ICharacterFactory factory;

    void Awake()
    {
        factory = factoryAsset as ICharacterFactory;
        if (factory == null) Debug.LogError("팩토리 에셋이 ICharacterFactory를 구현해야함.");
    }

    void Start()
    {
        // 팩토리로 플레이어와 적 생성
        factory.CreatePlayer(new Vector3(0, 0, 0));
        for (int i = 0; i < 5; i++)
            factory.CreateEnemy(new Vector3(2 * i, 0, 0));
    }
}
