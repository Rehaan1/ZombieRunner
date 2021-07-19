using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] float chaseRange = 5f;
    [SerializeField] float turnSpeed = 5f;

    Transform target;
    NavMeshAgent navMeshAgent;
    float distanceToTarget = Mathf.Infinity;
    bool isProvoked = false;
    EnemyHealth health;

    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        health = GetComponent<EnemyHealth>();
        target = FindObjectOfType<PlayerHealth>().transform;
    }

    void Update()
    {
        if(health.IsDead())
        {
            enabled = false;
            navMeshAgent.enabled = false;
        }

        distanceToTarget = Vector3.Distance(target.position, transform.position);
        if(isProvoked)
        {
            EngageTarget();

        }
        else if(distanceToTarget < chaseRange)
        {
            isProvoked = true;            
        }
 
    }

    public void OnDamageTaken()
    {
        isProvoked = true;
    }

    void EngageTarget()
    {
        FaceTarget();
        if(distanceToTarget >= navMeshAgent.stoppingDistance)
        {
            ChaseTarget();
        }

        if(distanceToTarget <= navMeshAgent.stoppingDistance)
        {
            AttackTarget();
        }
    }

    void ChaseTarget()
    {
        GetComponent<Animator>().SetBool("attack",false); //set the bool attack in animator
        GetComponent<Animator>().SetTrigger("move"); //trigger move parameter in animator
        navMeshAgent.SetDestination(target.position);  
    }

    void AttackTarget()
    {
        GetComponent<Animator>().SetBool("attack",true);
    }

    void FaceTarget()
    {
        Vector3 direction = (target.position - transform.position).normalized;  // normalized keeps the same direction but keeps vectors length to 1.0
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        
        // transform.rotation = where the target is, we need to rotate at a certain speed
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * turnSpeed);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, chaseRange);
    }
}
