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
    [Tooltip("감지할 레이어 (예: Enemy)")]
    public LayerMask detectionLayerMask;

    // Trigger Zone Collider는 'Is Trigger' 체크해야 합니다.
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
        // 1) 감지된 타겟 중 가장 가까운 적 찾기
        Transform nearest = null;
        float minDist = float.MaxValue;

        // 리스트가 비어 있으면 전진
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

        // 2) 공격 범위 체크 후 공격 또는 이동
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
        // 레이어로 적 필터
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
