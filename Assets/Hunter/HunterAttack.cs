using System;
using UnityEngine;
using UnityEngine.AI;

public class HunterAttack : StateMachineBehaviour
{
    Transform player;
    NavMeshAgent agent;
    Hunter hunter;

    // Called when the attack state starts
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Cache references
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        agent = animator.GetComponent<NavMeshAgent>();
        hunter = animator.GetComponent<Hunter>();

        // FIX: Use the cached and exposed reference from Hunter.cs
        if (hunter == null || hunter.AttackCollider == null)
        {
            Debug.LogError("Hunter script or AttackCollider not ready for HunterAttack state.", animator.gameObject);
            return;
        }

        // Enable the cached collider when attack starts
        hunter.AttackCollider.enabled = true;
    }

    // Called every frame while in the attack animation
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (hunter == null || player == null || agent == null) return;

        LookAtPlayer();

        // Stop attacking if player moves out of range
        float distanceFromPlayer = Vector3.Distance(player.position, animator.transform.position);
        if (distanceFromPlayer > hunter.stopAttackingRadius)
        {
            animator.SetBool("isAttacking", false);
        }
    }

    // Called when leaving the attack state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // FIX: Disable the cached collider when attack ends
        if (hunter != null && hunter.AttackCollider != null)
            hunter.AttackCollider.enabled = false;
    }

    private void LookAtPlayer()
    {
        // Make the hunter face the player only on the horizontal axis
        Vector3 direction = player.position - agent.transform.position;
        direction.y = 0; // Ignore vertical rotation

        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            agent.transform.rotation = targetRotation;
        }
    }
}