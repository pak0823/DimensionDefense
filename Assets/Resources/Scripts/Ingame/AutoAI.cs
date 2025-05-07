// AutoAI.cs
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

[RequireComponent(typeof(Collider2D))]
public class AutoAI : MonoBehaviour
{
    [Header("Detection")]
    [Tooltip("감지할 레이어")]
    public LayerMask detectionLayerMask;

    private List<Transform> detectedTargets = new List<Transform>();

    [Header("Attack Strategy")]
    public AttackStrategySO attackStrategy;

    private float lastAttackTime = 0f;

    private Character character;

    private void Awake()
    {
        // Character 컴포넌트 참조
        character = GetComponent<Character>();
    }

    private void Start()
    {
        // 전략 할당 확인
        if (attackStrategy == null)
            Debug.LogError("AutoAI: AttackStrategySO 에셋을 할당하세요.");
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

        float range = character.attackRange;
        bool inRange = (nearest != null && minDist <= range);

        // 2) 공격 범위 체크 후 공격 또는 이동
        // 쿨다운 체크
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
        // 적이 붙어 있는 부모 루트에서 Character 컴포넌트 찾기
        var charComp = other.GetComponentInParent<Character>();

        var dmg = other.GetComponentInParent<IDamageable>();

        // 레이어로 적 필터
        if (charComp != null && ((1 << other.gameObject.layer) & detectionLayerMask) != 0)
        {
            //Debug.Log("병사를 리스트에 추가함");
            detectedTargets.Add(other.transform);
        }
        else if(dmg != null && ((1 << other.gameObject.layer) & detectionLayerMask) != 0)
        {
            Debug.Log("건물을 리스트에 추가함");
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
