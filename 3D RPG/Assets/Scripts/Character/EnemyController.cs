using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyState { GUARD, PATROL, CHASE, DEAD }
[RequireComponent(typeof(NavMeshAgent))]
public class EnemyController : MonoBehaviour
{
    public EnemyState state;
    private NavMeshAgent agent;
    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }
    void Update()
    {
        SwitchState();
    }
    void SwitchState()
    {
        switch (state)
        {
            case EnemyState.GUARD:
            break;
            case EnemyState.PATROL:
            break;
            case EnemyState.CHASE:
            break;
            case EnemyState.DEAD:
            break;
        }
    }
}
