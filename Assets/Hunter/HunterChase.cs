using UnityEngine;
using UnityEngine.AI;

public class HunterChase : StateMachineBehaviour
{
    private NavMeshAgent agent;
    private Transform player;

    // Hunter movement and detection settings
    public float chaseSpeed = 5f;           // Faster than zombie
    public float detectionAreaRadius = 9999f; // Same as in HunterIdle (covers entire map)
    public float attackingDistance = 2.5f;   // Distance to start attacking

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Find player and agent
        player = GameObject.FindGameObjectWithTag("Player").transform;
        agent = animator.GetComponent<NavMeshAgent>();

        // Set the Hunter's chase speed
        if (agent != null)
            agent.speed = chaseSpeed;
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (player == null || agent == null) return;

        // Keep chasing player endlessly
        agent.SetDestination(player.position);
        animator.transform.LookAt(player);

        float distanceFromPlayer = Vector3.Distance(player.position, animator.transform.position);

        // Attack if close enough
        if (distanceFromPlayer < attackingDistance)
        {
            animator.SetBool("isAttacking", true);
        }

        // Optional: if the player somehow teleports extremely far away
        if (distanceFromPlayer > detectionAreaRadius)
        {
            // Stops chasing only if outside massive detection range
            animator.SetBool("isChasing", false);
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Stop movement when exiting chase
        if (agent != null)
            agent.SetDestination(animator.transform.position);
    }
}
