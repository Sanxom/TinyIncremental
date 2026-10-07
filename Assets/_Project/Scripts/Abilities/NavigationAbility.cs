using System;
using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class NavigationAbility : MonoBehaviour
{
    [Header("Components")]
    private NavMeshAgent agent;

    public Vector3 Velocity => agent.velocity;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    public bool TryGoTo(Vector3 targetPosition)
    {
        targetPosition.y = 0f;
        NavMeshPath path = new();
        bool isReachable = agent.CalculatePath(targetPosition, path);

        if (!isReachable)
        {
            Debug.LogError($"{agent} {gameObject.name} cannot reach the target position: {targetPosition}!");
            return false;
        }
        else
        {
            agent.SetPath(path);
            agent.isStopped = false;
            return true;
        }
    }

    public bool IsMoving()
    {
        if (agent.pathPending 
            || agent.hasPath && agent.remainingDistance > agent.stoppingDistance)
            return true;

        return agent.velocity.sqrMagnitude > 0f;
    }

    public bool HasReachedDestination()
    {
        if (agent.pathPending 
            || agent.remainingDistance > agent.stoppingDistance 
            || agent.hasPath && agent.velocity.sqrMagnitude != 0f)
            return false;

        return true;
    }

    public void Disable()
    {
        agent.enabled = false;
    }

    public void Enable()
    {
        //if (NavMesh.SamplePosition(transform.position, out NavMeshHit hit, 0.5f, NavMesh.AllAreas))
        //    agent.Warp(hit.position);

        agent.enabled = true;
    }
}