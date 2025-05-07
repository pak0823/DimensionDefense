// AutoAI.cs
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

[RequireComponent(typeof(Collider2D))]
public class AutoAI : MonoBehaviour
{
    [Header("Detection")]
    [Tooltip("������ ���̾�")]
    public LayerMask detectionLayerMask;

    private List<Transform> detectedTargets = new List<Transform>();

    [Header("Attack Strategy")]
    public AttackStrategySO attackStrategy;

    private float lastAttackTime = 0f;

    private Character character;

    private void Awake()
    {
        // Character ������Ʈ ����
        character = GetComponent<Character>();
    }

    private void Start()
    {
        // ���� �Ҵ� Ȯ��
        if (attackStrategy == null)
            Debug.LogError("AutoAI: AttackStrategySO ������ �Ҵ��ϼ���.");
    }

    void Update()
    {
        // 1) ������ Ÿ�� �� ���� ����� �� ã��
        Transform nearest = null;
        float minDist = float.MaxValue;

        // ����Ʈ�� ��� ������ ����
        if (detectedTargets.Count > 0)
        {
            foreach (var t in detectedTargets)
            {
                float d = Vector3.Distance(transform.position, t.position);
                if (d < minDist)
                {
                    minDist = d;
                    nearest = t;
                }
            }
        }

        float range = character.attackRange;
        bool inRange = (nearest != null && minDist <= range);

        // 2) ���� ���� üũ �� ���� �Ǵ� �̵�
        // ��ٿ� üũ
        if (inRange && Time.time >= lastAttackTime + character.attackCoolTime)
        {
            attackStrategy.Attack(gameObject, nearest.gameObject);
            lastAttackTime = Time.time;
        }
        else if (!inRange)
        {
            transform.Translate(Vector2.left * character.moveSpeed * Time.deltaTime);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // ���� �پ� �ִ� �θ� ��Ʈ���� Character ������Ʈ ã��
        var charComp = other.GetComponentInParent<Character>();

        var dmg = other.GetComponentInParent<IDamageable>();

        // ���̾�� �� ����
        if (charComp != null && ((1 << other.gameObject.layer) & detectionLayerMask) != 0)
        {
            //Debug.Log("���縦 ����Ʈ�� �߰���");
            detectedTargets.Add(other.transform);
        }
        else if(dmg != null && ((1 << other.gameObject.layer) & detectionLayerMask) != 0)
        {
            Debug.Log("�ǹ��� ����Ʈ�� �߰���");
            detectedTargets.Add(other.transform);
        }

    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (((1 << other.gameObject.layer) & detectionLayerMask) != 0)
        {
            detectedTargets.Remove(other.transform);
        }
    }
}
