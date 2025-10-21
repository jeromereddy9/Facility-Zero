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
    public float chaseSpeed = 5f;          // Speed during chase
    public float attackRadius = 2.5f;      // Distance to start attacking
    public float detectionRadius = 9999f;  // Endless detection
    public float stopChaseRadius = 9999f;  // Endless chase
    public float stopAttackingRadius = 2.5f; // Distance to stop attacking

    [Header("Death Effect")]
    public GameObject deathEffectPrefab; // Assign in Inspector
    
    public float deathAnimationDuration = 1.5f; 
    public float effectLifetime = 5f;     // How long the effect lasts

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
        else // Added warning if hand is not found
        {
            Debug.LogWarning("Hunter: Could not find 'HunterAttackHand' child object for attack collider.", this);
        }
    }

    public void TakeDamage(int damageAmount)
    {
        // Subtract damage from health
        mainHP -= damageAmount;

        if (mainHP <= 0)
        {
            // Trigger death animation
            if (animator != null) // Check if animator exists
            {
                animator.SetTrigger("DIE"); // Ensure "DIE" trigger exists in Animator Controller
            }
            else
            {
                Debug.LogError("Hunter: Animator component not found!", this);
            }

            // Disable components immediately
            Collider capCollider = GetComponent<CapsuleCollider>(); // Get collider directly
            if (capCollider != null) capCollider.enabled = false;
            if (navAgent != null) navAgent.enabled = false; // Check if navAgent exists
            if (attackCollider != null) attackCollider.enabled = false;

            // Start death effect sequence
            StartCoroutine(DeathSequence());
        }
        else
        {
            // Trigger damage animation if still alive
            if (animator != null) animator.SetTrigger("DAMAGE"); // Ensure "DAMAGE" trigger exists
        }
    }

    private IEnumerator DeathSequence()
    {
        // Wait for the duration of the death animation PLUS a small buffer
        // MODIFIED: Added a small extra delay (e.g., 0.1 seconds) ***
        yield return new WaitForSeconds(deathAnimationDuration + 1.0f);

        Debug.Log("Death animation finished (with buffer), triggering explosion.");

        // Spawn death effect at Hunter's position
        if (deathEffectPrefab != null)
        {
            GameObject effect = Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);
            Destroy(effect, effectLifetime); // Destroy effect after lifetime
        }
        else
        {
            Debug.LogWarning("Hunter: Death Effect Prefab not assigned.", this);
        }

        // Destroy Hunter object AFTER the delay and effect spawn
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