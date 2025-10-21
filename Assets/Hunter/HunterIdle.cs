using UnityEngine;

public class HunterIdle : StateMachineBehaviour
{
    private Transform player;
    public float detectionAreaRadius = 9999f; // Effectively covers the whole map

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (player == null) return;

        // Constantly check for player distance
        float distanceFromPlayer = Vector3.Distance(player.position, animator.transform.position);

        // Immediately transition to chase when detected
        if (distanceFromPlayer < detectionAreaRadius)
        {
            animator.SetBool("isChasing", true);
        }
    }
}


