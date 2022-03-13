using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyState { GUARD, PATROL, CHASE, DEAD }
[RequireComponent(typeof(NavMeshAgent))]
public class EnemyController : MonoBehaviour
{

    [Header("Basic Settings")]
    public float sightRadius = 8f;
    public bool isGuard;
    public float lookAtTime;
    private float remainLookAtTime;
    private float lastAttackTime;

    [Header("Patral State")]
    public float patralRange;

    private NavMeshAgent agent;
    private Animator anim;
    private CharacterStats characterStats;
    private EnemyState enemyState;
    private float speed;
    private GameObject attackTarget;
    private Vector3 wayPoint;
    private Vector3 guardPos;
    private Quaternion guardRotation;
    private Collider coll;

    private bool isWalk;
    private bool isChase;
    private bool isFollow;
    private bool isDead;
    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        characterStats = GetComponent<CharacterStats>();
        coll = GetComponent<Collider>();

        speed = agent.speed;
        guardPos = transform.position;
        guardRotation = transform.rotation;
        remainLookAtTime = lookAtTime;
        isDead = false;
    }

    void Start()
    {
        if (isGuard)
        {
            enemyState = EnemyState.GUARD;
        }
        else
        {
            enemyState = EnemyState.PATROL;
            GetNewWayPoint();
        }
    }

    void Update()
    {
        if (characterStats.CurrentHealth <= 0f)
        {
            isDead = true;
        }
        SwitchState();
        SetAnimator();
    }
    void SwitchState()
    {
        bool isFound = false;
        if (isDead)
        {
            enemyState = EnemyState.DEAD;
        }
        else
        {
            isFound = FoundPlayer();
            if (isFound)
            {
                enemyState = EnemyState.CHASE;
            }
        }

        switch (enemyState)
        {
            case EnemyState.GUARD:
                isChase = false;
                if (transform.position != guardPos)
                {
                    isWalk = true;
                    agent.isStopped = false;
                    agent.destination = guardPos;
                    if (Vector3.SqrMagnitude(guardPos - transform.position) <= agent.stoppingDistance)
                    {
                        isWalk = false;
                        transform.rotation = Quaternion.Lerp(transform.rotation, guardRotation, 0.01f);
                    }
                }
            break;
            case EnemyState.PATROL:
                isChase = false;
                agent.speed = speed * 0.5f; // 乘法性能开销比除法要小

                if (Vector3.Distance(transform.position, wayPoint) <= agent.stoppingDistance)
                {
                    if (remainLookAtTime > 0f)
                    {
                        isWalk = false;
                        remainLookAtTime -= Time.deltaTime;
                    }
                    else
                    {
                        GetNewWayPoint();
                        remainLookAtTime = lookAtTime;
                    }
                }
                else
                {
                    agent.isStopped = false;
                    agent.destination = wayPoint;
                    isWalk = true;
                }

            break;
            case EnemyState.CHASE:
                isWalk = false;
                isChase = true;
                agent.speed = speed;

                if (isFound)
                {
                    isFollow = true;
                    agent.isStopped = false;
                    agent.destination = attackTarget.transform.position;
                }
                else
                {
                    isFollow = false;
                    agent.destination = transform.position;
                    if (remainLookAtTime > 0f)
                    {
                        remainLookAtTime -= Time.deltaTime;
                    }
                    else
                    {
                        enemyState = isGuard ? EnemyState.GUARD : EnemyState.PATROL;
                    }
                }
                if (TargetInAttackRange() || TargetInSkillRange())
                {
                    isFollow = false;
                    agent.isStopped = true;
                    if (Time.time - lastAttackTime > characterStats.attackData.coolDown)
                    {
                        lastAttackTime = Time.time;
                        // 执行攻击
                        Attack();
                    }
                }
            break;
            case EnemyState.DEAD:
                coll.enabled = false;
                agent.enabled = false;
                Destroy(gameObject, 2f);
            break;
        }
    }
    void Attack()
    {
        transform.LookAt(attackTarget.transform);
        if (TargetInAttackRange())
        {
            characterStats.isCritical = UnityEngine.Random.value < characterStats.attackData.criticalChance;
            // 近身攻击
            anim.SetBool("Critical", characterStats.isCritical);
            anim.SetTrigger("Attack");
        }
        else if (TargetInSkillRange())
        {
            // 技能攻击
            anim.SetTrigger("Skill");
        }
    }
    bool FoundPlayer()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position,
            sightRadius, 1 << LayerMask.NameToLayer("Player"));
        
        foreach (var collier in colliders)
        {
            if (collier.CompareTag("Player"))
            {
                attackTarget = collier.gameObject;
                return true;
            }
        }
        return false;
    }
    void SetAnimator()
    {
        anim.SetBool("Walk", isWalk);
        anim.SetBool("Chase", isChase);
        anim.SetBool("Follow", isFollow);
        anim.SetBool("Dead", isDead);
    }

    // 仅当物体选中时才会画出来
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, sightRadius);
        if (agent != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(wayPoint, agent.stoppingDistance * 0.5f);
        }
    }

    void GetNewWayPoint()
    {
        float randomX = Random.Range(-patralRange, patralRange);
        float randomZ = Random.Range(-patralRange, patralRange);
        Vector3 randomPoint = new Vector3(guardPos.x + randomX, transform.position.y, guardPos.z + randomZ);
        NavMeshHit hit;
        // 1 表示 walkable层
        wayPoint = NavMesh.SamplePosition(randomPoint, out hit, agent.stoppingDistance, 1) ? hit.position : transform.position;
    }

    bool TargetInAttackRange()
    {
        if (attackTarget != null)
        {
            return Vector3.Distance(attackTarget.transform.position, transform.position) <= characterStats.attackData.attackRange;
        }
        return false;
    }

    bool TargetInSkillRange()
    {
        if (attackTarget != null)
        {
            return Vector3.Distance(attackTarget.transform.position, transform.position) <= characterStats.attackData.skillRange;
        }
        return false;
    }

    // Animation Event
    void Hit()
    {
        if (attackTarget != null)
        {
            var targetStats = attackTarget.GetComponent<CharacterStats>();
            targetStats.TakeDamage(characterStats, targetStats);
        }
    }
}
