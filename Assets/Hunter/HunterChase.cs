using UnityEngine;
using UnityEngine.AI;

public class HunterChase : StateMachineBehaviour
{
    private NavMeshAgent agent;
    private Transform player;
    private Hunter hunter; // Reference to the Hunter script

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Cache references
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        agent = animator.GetComponent<NavMeshAgent>();
        hunter = animator.GetComponent<Hunter>(); // Get the Hunter component

        if (agent == null || hunter == null)
        {
            Debug.LogError("NavMeshAgent or Hunter script not found on the Animator's GameObject.", animator.gameObject);
            return;
        }

        // FIX: Use the dynamically scaled speed from the Hunter script
        agent.speed = hunter.chaseSpeed;
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (player == null || agent == null || hunter == null) return;

        // Keep chasing player
        agent.SetDestination(player.position);

        // Face the player on the horizontal plane
        Vector3 direction = player.position - animator.transform.position;
        direction.y = 0;
        if (direction != Vector3.zero)
        {
            animator.transform.rotation = Quaternion.LookRotation(direction);
        }

        float distanceFromPlayer = Vector3.Distance(player.position, animator.transform.position);

        // Attack if close enough (Using hunter.attackRadius for dynamic setting)
        if (distanceFromPlayer < hunter.attackRadius)
        {
            animator.SetBool("isAttacking", true);
        }

        // Check if player is outside of the range where the Hunter should stop chasing
        if (distanceFromPlayer > hunter.stopChaseRadius)
        {
            animator.SetBool("isChasing", false);
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Stop movement when exiting chase (important for the attack state to take over)
        if (agent != null)
            agent.SetDestination(animator.transform.position);
    }
}