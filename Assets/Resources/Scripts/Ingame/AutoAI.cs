// AutoAI.cs
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class AutoAI : MonoBehaviour
{
    [Header("Detection")]
    [Tooltip("������ ���̾�")]
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
        // Character ������Ʈ ����
        character = GetComponent<Character>();
        animator = GetComponentInChildren<Animator>();
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
        else if (inRange)    // 2) ���� ���� üũ �� ���� �Ǵ� �̵�
        {
            if(Time.time >= lastAttackTime + character.attackCoolTime)// ��ٿ� üũ
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
        // 1) ���̾� ���͸�
        int mask = 1 << other.gameObject.layer;
        if (((1 << other.gameObject.layer) & detectionLayerMask) == 0) return;

        // 2) ������Ʈ �Ǻ�
        var otherChar = other.GetComponentInParent<Character>();
        var otherbuilding = other.GetComponentInParent<IDamageable>();

        if (otherChar != null)
        {
            // �� ����(thisCharacter.definition.isEnemy)�� �ٸ� �����̸� ���� �������
            bool isEnemyTarget = otherChar.definition.isEnemy
                                 != this.character.definition.isEnemy;
            if (isEnemyTarget)
            {
                detectedTargets.Add(other.transform);
            }
            // ���� �����̰�, �� AI�� buffStrategy�� ������ ���� ���� ���� �������
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
            // Character�� �ƴ�����, IDamageable�̸�(��: ����) �� �������
            detectedTargets.Add(other.transform);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (((1 << other.gameObject.layer) & detectionLayerMask) != 0)
        {
            detectedTargets.Remove(other.transform);
            detectedBuffTargets.Remove(other.transform);
            //Debug.Log("����Ʈ���� ����: " + other.name);
        }
    }

    public void ForceRemoveTarget(Transform t)
    {
        detectedTargets.Remove(t);
    }
}
