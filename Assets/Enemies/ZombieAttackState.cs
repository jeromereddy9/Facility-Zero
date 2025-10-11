using System;
using UnityEngine;
using UnityEngine.AI;

public class ZombieAttackState : StateMachineBehaviour
{
    Transform player;
    NavMeshAgent agent;
    SphereCollider attackCollider; // explicitly use SphereCollider

    public float stopAttackingDistance = 2.5f;

    // Called when the attack state starts
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Cache player reference
        player = GameObject.FindGameObjectWithTag("Player").transform;
        agent = animator.GetComponent<NavMeshAgent>();

        // Find the hand with tag "ZombieAttackHand" within THIS enemy only
        Transform[] children = animator.GetComponentsInChildren<Transform>();
        foreach (Transform child in children)
        {
            if (child.CompareTag("ZombieAttackHand"))
            {
                attackCollider = child.GetComponent<SphereCollider>();
                break;
            }
        }

        // Enable collider when attack starts
        if (attackCollider != null)
            attackCollider.enabled = true;
    }

    // Called each frame while in the attack animation
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        LookAtPlayer();

        float distanceFromPlayer = Vector3.Distance(player.position, animator.transform.position);
        if (distanceFromPlayer > stopAttackingDistance)
        {
            animator.SetBool("isAttacking", false);
        }
    }

    // Called when attack animation ends
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Disable collider when leaving attack state
        if (attackCollider != null)
            attackCollider.enabled = false;
    }

    private void LookAtPlayer()
    {
        // Make sure the zombie faces the player horizontally only
        Vector3 direction = player.position - agent.transform.position;
        agent.transform.rotation = Quaternion.LookRotation(direction);

        float yRotation = agent.transform.eulerAngles.y;
        agent.transform.rotation = Quaternion.Euler(0, yRotation, 0);
    }
}
