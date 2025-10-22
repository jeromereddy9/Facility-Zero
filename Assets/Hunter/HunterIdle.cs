using UnityEngine;

public class HunterIdle : StateMachineBehaviour
{
    private Transform player;
    private Hunter hunter; // ADDED: Reference to the Hunter script

    // Removed: public float detectionAreaRadius = 9999f; // Get this from Hunter.cs

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        hunter = animator.GetComponent<Hunter>(); // ADDED: Get Hunter component

        if (hunter == null)
        {
            Debug.LogError("Hunter script not found on the Animator's GameObject.", animator.gameObject);
        }
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (player == null || hunter == null) return;

        // Constantly check for player distance
        float distanceFromPlayer = Vector3.Distance(player.position, animator.transform.position);

        // Immediately transition to chase when detected.
        // FIX: Use the detectionRadius variable from the Hunter component.
        if (distanceFromPlayer < hunter.detectionRadius)
        {
            animator.SetBool("isChasing", true);
        }
    }

    // OnStateExit is not needed since the agent is not moving in Idle.
}