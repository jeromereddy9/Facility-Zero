using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombiePatrollingState : StateMachineBehaviour
{
    float timer;
    public float patrollingTime = 10f;

    Transform player;
    NavMeshAgent agent;

    public float detectionArea = 18f;
    public float patrolSpeed = 2f;

    List<Transform> wayPointsList = new List<Transform>();

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Initialisation //

        player = GameObject.FindGameObjectWithTag("Player").transform;
        agent = animator.GetComponent<NavMeshAgent>();

        agent.speed = patrolSpeed;
        timer = 0;

        // Get all waypoints and Move to First Waypoint //

        GameObject waypointCluster = GameObject.FindGameObjectWithTag("Waypoints");
        foreach(Transform t in waypointCluster.transform)
        {
            wayPointsList.Add(t);
        }

        Vector3 nextPosition = wayPointsList[Random.Range(0, wayPointsList.Count)].position;
        agent.SetDestination(nextPosition);
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // If  agent arrived at waypoint, move to another waypoint //

        if(agent.remainingDistance <= agent.stoppingDistance)
        {
            agent.SetDestination(wayPointsList[Random.Range(0, wayPointsList.Count)].position);
        }

        // Transition to Idle state // 

        timer += Time.deltaTime;

        if(timer > patrollingTime)
        {
            animator.SetBool("isPatrolling", true);
        }

        // Transition to Chase State //

        float distanceFromPlayer = Vector3.Distance(player.position, animator.transform.position);

        if (distanceFromPlayer < detectionArea)
        {
            animator.SetBool("isChasing", true);
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Stop the agent //
        agent.SetDestination(agent.transform.position);
    }
}
