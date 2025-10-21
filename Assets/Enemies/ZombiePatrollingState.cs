using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombiePatrollingState : StateMachineBehaviour
{
    float timer;
    public float patrollingTime = 10f;

    Transform player;
    NavMeshAgent agent;

    public float detectionArea = 14f;
    public float patrolSpeed = 4f;

    List<Transform> wayPointsList = new List<Transform>();

    // Each enemy has its own cluster (assign this in Inspector for Enemy)
    Transform waypointCluster;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Initialisation //
        player = GameObject.FindGameObjectWithTag("Player").transform;
        agent = animator.GetComponent<NavMeshAgent>();

        agent.speed = patrolSpeed;
        timer = 0;

        // Get cluster reference from Enemy script
        Enemy enemy = animator.GetComponent<Enemy>();
        if (enemy != null)
        {
            waypointCluster = enemy.waypointCluster;
        }


        // Clear and repopulate waypoints from this enemy's cluster //
        wayPointsList.Clear();

        if (waypointCluster != null)
        {
            foreach (Transform t in waypointCluster)
            {
                wayPointsList.Add(t);
            }

            // Move to first waypoint
            if (wayPointsList.Count > 0)
            {
                Vector3 nextPosition = wayPointsList[Random.Range(0, wayPointsList.Count)].position;
                agent.SetDestination(nextPosition);
            }
        }
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (wayPointsList.Count == 0) return;

        // If agent arrived at waypoint, move to another waypoint //
        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            agent.SetDestination(wayPointsList[Random.Range(0, wayPointsList.Count)].position);
        }

        // Transition to Idle state // 
        timer += Time.deltaTime;
        if (timer > patrollingTime)
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
