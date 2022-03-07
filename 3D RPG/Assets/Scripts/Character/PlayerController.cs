using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private float attackDuration = 1f;
    private NavMeshAgent agent;
    private Animator anim;
    private GameObject attackTarget;
    private float lastAttackTime;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        anim.SetFloat("Speed", agent.velocity.sqrMagnitude); // todo 0到1
    }

    void Start()
    {
        MouseManager.Instance.OnMouseClickEvent += MoveToTarget;
        MouseManager.Instance.OnEnemyClickEvent += MoveToAttackTarget;
    }

    void MoveToTarget(Vector3 target)
    {
        StopAllCoroutines();
        agent.isStopped = false;
        agent.destination = target;
    }

    private void MoveToAttackTarget(GameObject target)
    {
        if (target != null)
        {
            attackTarget = target;
            StartCoroutine(MoveToAttackTarget());
        }
    }

    IEnumerator MoveToAttackTarget()
    {
        agent.isStopped = false;
        while (Vector3.Distance(transform.position, attackTarget.transform.position) > 2)
        {
            agent.destination = attackTarget.transform.position;
            yield return null;
        }
        agent.isStopped = true;
        if (Time.time - lastAttackTime > attackDuration)
        {
            transform.LookAt(attackTarget.transform);
            lastAttackTime = Time.time;
            anim.SetTrigger("Attack");
        }
    }
}
