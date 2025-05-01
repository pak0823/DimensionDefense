// AutoAI.cs
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class AutoAI : MonoBehaviour
{
    [Header("Movement & Attack")]
    public float moveSpeed = 3f;
    public float attackRange = 1.5f;

    [Header("Detection")]
    [Tooltip("������ ���̾� (��: Enemy)")]
    public LayerMask detectionLayerMask;

    // Trigger Zone Collider�� 'Is Trigger' üũ�ؾ� �մϴ�.
    private List<Transform> detectedTargets = new List<Transform>();

    [Header("Attack Strategy")]
    [SerializeField] private AttackStrategySO attackStrategy;

    [Header("Attack Strategy")]
    Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
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

        // 2) ���� ���� üũ �� ���� �Ǵ� �̵�
        if (nearest != null && minDist <= attackRange)
        {
            attackStrategy.Attack(gameObject, nearest.gameObject);
        }
        else
        {
            transform.Translate(Vector2.left * moveSpeed * Time.deltaTime);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // ���̾�� �� ����
        if (((1 << other.gameObject.layer) & detectionLayerMask) != 0)
        {
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
