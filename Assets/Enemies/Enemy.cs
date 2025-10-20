using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [SerializeField] private int HP = 100;
    private Animator animator;
    private NavMeshAgent navAgent;

    public int CurrentHP { get { return HP; } set { HP = value; } }
    public int id;

    public Transform waypointCluster;

    private SphereCollider attackCollider;

    private void Start()
    {
        animator = GetComponent<Animator>();
        navAgent = GetComponent<NavMeshAgent>();

        // Find the attack hand collider (sphere collider)
        Transform hand = transform.Find("ZombieAttackHand");
        if (hand != null)
        {
            attackCollider = hand.GetComponent<SphereCollider>();
            if (attackCollider != null)
                attackCollider.enabled = false; // Ensure it starts off
        }
    }

    public void TakeDamage(int damageAmount)
    {
        HP -= damageAmount;

        if (HP <= 0)
        {
            animator.SetTrigger("DIE");
            GetComponent<CapsuleCollider>().enabled = false;
            navAgent.enabled = false;

            // Disable the attack collider too
            if (attackCollider != null)
                attackCollider.enabled = false;

            // Start coroutine to destroy the enemy after 20 seconds
            StartCoroutine(DestroyAfterDelay(20f));
        }
        else
        {
            animator.SetTrigger("DAMAGE");
        }
    }

    private IEnumerator DestroyAfterDelay(float delay)
    {
        // Wait for the delay (zombie lies on the ground)
        yield return new WaitForSeconds(delay);

        // Safely remove the enemy GameObject from the scene
        Destroy(gameObject);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 2.5f); // Attacking / Stop Attacking

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, 12f); // Detection (Start Chasing)

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, 14f); // Stop Chasing
    }
}
