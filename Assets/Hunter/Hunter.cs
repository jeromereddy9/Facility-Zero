using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Hunter : MonoBehaviour
{
    // Static Variables for Respawn & Scaling
    private static int deathCount = 0;
    private static float currentRespawnDelay = 10f; // Default initial delay
    private static float currentChaseSpeed = 5f;   // Default initial speed
    private static int currentAttackDamage = 10;   // Default initial damage
    private static bool isRespawning = false;      // Flag to prevent multiple respawn coroutines

    // Fields for respawn scaling
    [Header("Respawn Settings")] 
    [Tooltip("Tag used on empty GameObjects for respawn locations.")]
    [SerializeField] private string spawnPointTag = "HunterSpawnPoint";
    [SerializeField] private float initialRespawnDelay = 10f;
    [SerializeField] private float respawnDelayIncrease = 2f;
    [SerializeField] private float maxRespawnDelay = 60f;
    [SerializeField] private float initialChaseSpeed = 5f;
    [SerializeField] private float chaseSpeedIncrease = 0.5f;
    [SerializeField] private int initialAttackDamage = 10;
    [SerializeField] private int attackDamageIncrease = 2;


    [Header("Hunter Health Settings")]
    [SerializeField] private int initialHP = 50; 
    private int mainHP; // Current health

    private Animator animator;
    private NavMeshAgent navAgent;
    private SphereCollider attackCollider;
    private Collider capCollider; 

    [Header("Hunter AI Settings")]
    public float chaseSpeed = 5f;          // Speed during chase (will be updated by scaling)
    public float attackRadius = 2.5f;      // Distance to start attacking
    public float detectionRadius = 9999f;  // Endless detection
    public float stopChaseRadius = 9999f;  // Endless chase
    public float stopAttackingRadius = 2.5f; // Distance to stop attacking
    public int attackDamage = 10; // Attack damage

    [Header("Death Effect")]
    public GameObject deathEffectPrefab; // Assign in Inspector

    public float deathAnimationDuration = 1.5f;
    public float effectLifetime = 5f;     // How long the effect lasts

    private bool isDead = false; // Flag for death state
    private static MonoBehaviour coroutineRunner; // Static reference for coroutines


    private void Awake() 
    {
        EnsureCoroutineRunner();

        animator = GetComponent<Animator>();
        navAgent = GetComponent<NavMeshAgent>();
        capCollider = GetComponent<CapsuleCollider>();

        // Find Hunter’s hand collider logic remains
        Transform hand = transform.Find("HunterAttackHand");
        if (hand != null)
        {
            attackCollider = hand.GetComponent<SphereCollider>();
            if (attackCollider != null)
                attackCollider.enabled = false;
        }
        else
        {
            Debug.LogWarning("Hunter: Could not find 'HunterAttackHand' child object for attack collider.", this);
        }

        // Set initial stats only once when the game starts
        if (deathCount == 0)
        {
            currentChaseSpeed = initialChaseSpeed;
            currentAttackDamage = initialAttackDamage;
            currentRespawnDelay = initialRespawnDelay;
        }
    }

    // OnEnable method to handle reset logic
    private void OnEnable()
    {
        mainHP = initialHP; // Reset Health
        isDead = false;

        // Re-enable components
        if (capCollider != null) capCollider.enabled = true;
        if (navAgent != null)
        {
            if (!navAgent.enabled) navAgent.enabled = true;
            navAgent.isStopped = false;

            // Positioning logic
            GameObject[] spawnPointObjects = GameObject.FindGameObjectsWithTag(spawnPointTag);
            if (spawnPointObjects.Length > 0)
            {
                int spawnIndex = Random.Range(0, spawnPointObjects.Length);
                Transform selectedSpawnPoint = spawnPointObjects[spawnIndex].transform;
                if (navAgent.isOnNavMesh)
                {
                    navAgent.Warp(selectedSpawnPoint.position);
                    transform.rotation = selectedSpawnPoint.rotation;
                }
            }
            navAgent.ResetPath();
        }
        if (attackCollider != null) attackCollider.enabled = false;

        // Reset Animator
        if (animator != null)
        {
            animator.ResetTrigger("DIE");
            animator.ResetTrigger("DAMAGE");
            animator.Play("Idle", 0, 0f); // Force back to Idle state
        }

        ApplyCurrentStats(); // Apply potentially scaled stats
        Debug.Log($"Hunter {gameObject.name} Enabled/Respawned.");
    }

    // Apply stats to the instance
    private void ApplyCurrentStats()
    {
        this.chaseSpeed = currentChaseSpeed;
        this.attackDamage = currentAttackDamage;
        if (navAgent != null && navAgent.enabled)
        {
            navAgent.speed = this.chaseSpeed;
        }
    }

    public void TakeDamage(int damageAmount)
    {
        if (isDead) return; // Check to prevent multiple deaths

        mainHP -= damageAmount;

        if (mainHP <= 0)
        {
            isDead = true; // Set flag

            if (animator != null)
            {
                animator.SetTrigger("DIE");
            }
            else
            {
                Debug.LogError("Hunter: Animator component not found!", this);
            }

            // Disable components (using cached reference for main collider)
            if (capCollider != null) capCollider.enabled = false;
            if (navAgent != null) navAgent.enabled = false;
            if (attackCollider != null) attackCollider.enabled = false;

            StartCoroutine(DeathSequence());
        }
        else
        {
            if (animator != null) animator.SetTrigger("DAMAGE");
        }
    }

    private IEnumerator DeathSequence()
    {
        // Wait for the duration of the death animation PLUS your original buffer
        yield return new WaitForSeconds(deathAnimationDuration + 1.0f);

        Debug.Log("Death animation finished (with buffer), triggering explosion.");

        if (deathEffectPrefab != null)
        {
            GameObject effect = Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);
            Destroy(effect, effectLifetime);
        }
        else
        {
            Debug.LogWarning("Hunter: Death Effect Prefab not assigned.", this);
        }

        // Prepare respawn and disable self instead of destroying
        PrepareRespawn(this);
        gameObject.SetActive(false);
    }

    // Static Respawn Logic
    private static void PrepareRespawn(Hunter sourceSettingsHunter)
    {
        if (isRespawning) return;
        deathCount++;

        if (sourceSettingsHunter == null)
        {
            Debug.LogError("CRITICAL: Source Hunter for settings is null!");
            return;
        }

        // Calculate next stats
        currentRespawnDelay = Mathf.Min(sourceSettingsHunter.initialRespawnDelay + (deathCount * sourceSettingsHunter.respawnDelayIncrease), sourceSettingsHunter.maxRespawnDelay);
        currentChaseSpeed = sourceSettingsHunter.initialChaseSpeed + (deathCount * sourceSettingsHunter.chaseSpeedIncrease);
        currentAttackDamage = sourceSettingsHunter.initialAttackDamage + (deathCount * sourceSettingsHunter.attackDamageIncrease);

        Debug.Log($"Hunter died! Death count: {deathCount}. Next respawn in {currentRespawnDelay}s. Next Speed: {currentChaseSpeed}, Next Damage: {currentAttackDamage}");

        if (coroutineRunner != null)
        {
            isRespawning = true;
            coroutineRunner.StartCoroutine(RespawnHunterCoroutine(currentRespawnDelay, sourceSettingsHunter));
        }
        else
        {
            Debug.LogError("Coroutine Runner is null! Cannot start respawn timer.");
        }
    }

    private static IEnumerator RespawnHunterCoroutine(float delay, Hunter hunterToRespawn)
    {
        yield return new WaitForSeconds(delay);
        if (hunterToRespawn != null)
        {
            hunterToRespawn.gameObject.SetActive(true);
        }
        else
        {
            Debug.LogError("Hunter instance to respawn was destroyed!");
        }
        isRespawning = false;
    }

    // Helper to get the coroutine runner
    private void EnsureCoroutineRunner()
    {
        if (coroutineRunner == null)
        {
            GameObject runnerGO = GameObject.Find("HunterCoroutineRunner");
            if (runnerGO == null) runnerGO = new GameObject("HunterCoroutineRunner");
            DontDestroyOnLoad(runnerGO); // Make sure it persists
            coroutineRunner = runnerGO.GetComponent<EmptyMonoBehaviour>() ?? runnerGO.AddComponent<EmptyMonoBehaviour>();
        }
    }


    private void OnDrawGizmos()
    {
        // This method remains unchanged
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRadius); // Attack range

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, detectionRadius); // Detection / Chase start

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, stopChaseRadius); // Chase stop
    }
}

// Dummy MonoBehaviour needed for running coroutines statically ---
public class EmptyMonoBehaviour : MonoBehaviour { }