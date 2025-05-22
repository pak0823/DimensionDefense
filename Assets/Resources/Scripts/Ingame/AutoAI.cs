// AutoAI.cs
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class AutoAI : MonoBehaviour
{
    [Header("Detection")]
    [Tooltip("감지할 레이어")]
    public LayerMask detectionLayerMask;

    private List<Transform> detectedTargets = new List<Transform>();
    private List<Transform> detectedBuffTargets = new List<Transform>();

    [Header("Attack Strategy")]
    public AttackStrategySO attackStrategy;
    public AttackStrategySO subAttackStrategy;
    public BuffAttack BuffAttack;

    private float lastAttackTime = 0f;

    private Character character;
    private Animator animator;

    private void Awake()
    {
        // Character 컴포넌트 참조
        character = GetComponent<Character>();
        animator = GetComponentInChildren<Animator>();
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

        bool inRange = (nearest != null);
        bool inBuffRange = detectedBuffTargets.Count > 0;

        if (BuffAttack != null && inBuffRange
        && Time.time >= lastAttackTime + character.attackCoolTime)
        {
            var ally = detectedBuffTargets
            .OrderBy(t => Vector3.Distance(transform.position, t.position))
            .First();
            BuffAttack.Attack(gameObject, ally.gameObject);
            lastAttackTime = Time.time;
            animator.SetBool("Move", false);
        }
        else if (inRange)    // 2) 공격 범위 체크 후 공격 또는 이동
        {
            if(Time.time >= lastAttackTime + character.attackCoolTime)// 쿨다운 체크
            {
                attackStrategy.Attack(gameObject, nearest.gameObject);

                if(subAttackStrategy != null)
                    subAttackStrategy.Attack(gameObject, nearest.gameObject);

                lastAttackTime = Time.time;
            }

            animator.SetBool("Move", false);
        }
        else
        {
            transform.Translate(Vector2.left * character.moveSpeed * Time.deltaTime);
            animator.SetBool("Move", true);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // 1) 레이어 필터링
        int mask = 1 << other.gameObject.layer;
        if (((1 << other.gameObject.layer) & detectionLayerMask) == 0) return;

        // 2) 컴포넌트 판별
        var otherChar = other.GetComponentInParent<Character>();
        var otherbuilding = other.GetComponentInParent<IDamageable>();

        if (otherChar != null)
        {
            // 내 진영(thisCharacter.definition.isEnemy)과 다른 진영이면 공격 대상으로
            bool isEnemyTarget = otherChar.definition.isEnemy
                                 != this.character.definition.isEnemy;
            if (isEnemyTarget)
            {
                detectedTargets.Add(other.transform);
            }
            // 같은 진영이고, 내 AI가 buffStrategy를 가지고 있을 때만 버프 대상으로
            else if (otherChar.definition.isEnemy
                  == this.character.definition.isEnemy
                  && this.character.characterType.Equals(5) || this.character.characterType.Equals(6))
            {
                detectedBuffTargets.Add(other.transform);
            }

            return;
        }
        else if (otherbuilding != null)
        {
            // Character는 아니지만, IDamageable이면(예: 기지) 적 대상으로
            detectedTargets.Add(other.transform);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (((1 << other.gameObject.layer) & detectionLayerMask) != 0)
        {
            detectedTargets.Remove(other.transform);
            detectedBuffTargets.Remove(other.transform);
            //Debug.Log("리스트에서 삭제: " + other.name);
        }
    }

    public void ForceRemoveTarget(Transform t)
    {
        detectedTargets.Remove(t);
    }
}
