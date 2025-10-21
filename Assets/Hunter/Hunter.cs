using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Hunter : MonoBehaviour
{
    [Header("Hunter Health Settings")]
    [SerializeField] private int mainHP = 50; // Total Hunter health

    private Animator animator;
    private NavMeshAgent navAgent;
    private SphereCollider attackCollider;

    [Header("Hunter AI Settings")]
    public float chaseSpeed = 5f;             // Speed during chase
    public float attackRadius = 2.5f;         // Distance to start attacking
    public float detectionRadius = 9999f;     // Endless detection
    public float stopChaseRadius = 9999f;     // Endless chase
    public float stopAttackingRadius = 2.5f;  // Distance to stop attacking

    [Header("Death Effect")]
    public GameObject deathEffectPrefab; // Assign in Inspector
    public float effectDelay = 1f;     // Delay to sync with animation
    public float effectLifetime = 5f;    // How long the effect lasts

    private void Start()
    {
        animator = GetComponent<Animator>();
        navAgent = GetComponent<NavMeshAgent>();

        // Find Hunter’s hand collider
        Transform hand = transform.Find("HunterAttackHand");
        if (hand != null)
        {
            attackCollider = hand.GetComponent<SphereCollider>();
            if (attackCollider != null)
                attackCollider.enabled = false; // Start disabled
        }
    }

    public void TakeDamage(int damageAmount)
    {
        // Subtract damage from health
        mainHP -= damageAmount;

        if (mainHP <= 0)
        {
            // Trigger death animation
            animator.SetTrigger("DIE");
            GetComponent<CapsuleCollider>().enabled = false;
            navAgent.enabled = false;

            if (attackCollider != null)
                attackCollider.enabled = false;

            // Start death effect sequence
            StartCoroutine(DeathSequence());
        }
        else
        {
            // Trigger damage animation if still alive
            animator.SetTrigger("DAMAGE");
        }
    }

    private IEnumerator DeathSequence()
    {
        // Wait a short delay for death animation
        yield return new WaitForSeconds(effectDelay);

        // Spawn death effect at Hunter's position
        if (deathEffectPrefab != null)
        {
            GameObject effect = Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);
            Destroy(effect, effectLifetime); // Destroy effect after lifetime
        }

        // Destroy Hunter object
        Destroy(gameObject);
    }

    private void OnDrawGizmos()
    {
        // Visual debug ranges
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRadius); // Attack range

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, detectionRadius); // Detection / Chase start

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, stopChaseRadius); // Chase stop
    }
}


