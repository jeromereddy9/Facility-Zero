using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Hunter : MonoBehaviour
{
    // --- ADDED: Static Variables for Respawn & Scaling ---
    private static int deathCount = 0;
    private static float currentRespawnDelay = 10f; 
    private static float currentChaseSpeed = 5f;   
    private static int currentAttackDamage = 10;   
    private static bool isRespawning = false;      

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
    
    // --- MODIFIED: Private backing field and Public Property for Attack Collider ---
    private SphereCollider attackColliderInternal; 
    private Rigidbody rb; // ADDED: Cached Rigidbody
    
    // Public access for HunterAttack script to use
    public SphereCollider AttackCollider => attackColliderInternal;
    // -------------------------------------------------------------------------------
    
    private Collider capCollider; // Stored reference for main collider
    private GameObject attackHandGO; // Stored reference for attack hand GameObject


    [Header("Hunter AI Settings")]
    public float chaseSpeed = 5f;      
    public float attackRadius = 2.5f;  
    public float detectionRadius = 9999f; 
    public float stopChaseRadius = 9999f; 
    public float stopAttackingRadius = 2.5f; 
    public int attackDamage = 10; 

    [Header("Death Effect")]
    public GameObject deathEffectPrefab; 

    public float deathAnimationDuration = 1.5f;
    public float effectLifetime = 5f;      

    private bool isDead = false; 
    private static MonoBehaviour coroutineRunner; 


    private void Awake()
    {
        EnsureCoroutineRunner();

        rb = GetComponent<Rigidbody>(); // CACHED Rigidbody
        animator = GetComponent<Animator>();
        navAgent = GetComponent<NavMeshAgent>();
        capCollider = GetComponent<CapsuleCollider>(); 

        // Find Hunter’s hand collider logic remains
        Transform hand = transform.Find("HunterAttackHand");
        if (hand != null)
        {
            attackHandGO = hand.gameObject;
            // ASSIGNED TO INTERNAL FIELD
            attackColliderInternal = hand.GetComponent<SphereCollider>(); 
            if (attackColliderInternal != null)
                attackColliderInternal.enabled = false;
        }
        else
        {
            Debug.LogWarning("Hunter: Could not find 'HunterAttackHand' child object for attack collider.", this);
        }

        if (deathCount == 0)
        {
            currentChaseSpeed = initialChaseSpeed;
            currentAttackDamage = initialAttackDamage;
            currentRespawnDelay = initialRespawnDelay;
        }
    }

    private void OnEnable()
    {
        mainHP = initialHP; 
        isDead = false;

        // Re-enable main collider
        if (capCollider != null) capCollider.enabled = true;

        if (rb != null)
        {
            // FIX: Reset Rigidbody state completely (Prevents "push away" from physics glitches)
            rb.isKinematic = true;  
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = false; 
            rb.detectCollisions = true; 
        }
        
        if (navAgent != null)
        {
            // 1. Force a stop and ensure enabled before attempting reset
            navAgent.isStopped = true;
            if (!navAgent.enabled) navAgent.enabled = true;
            
            // 2. Positioning/Warp Logic
            GameObject[] spawnPointObjects = GameObject.FindGameObjectsWithTag(spawnPointTag);
            if (spawnPointObjects.Length > 0)
            {
                int spawnIndex = Random.Range(0, spawnPointObjects.Length);
                Transform selectedSpawnPoint = spawnPointObjects[spawnIndex].transform;

                // FIX: Hard reset the agent position
                navAgent.enabled = false; 
                transform.position = selectedSpawnPoint.position;
                transform.rotation = selectedSpawnPoint.rotation;
                navAgent.enabled = true; 
            }
            
            // 3. Clear any residual path/movement
            navAgent.ResetPath();
            navAgent.isStopped = false; 
        }

        // FIX: Explicitly enable the attack hand GameObject on respawn
        if (attackHandGO != null) attackHandGO.SetActive(true);
        
        // Ensure the collider component is enabled on its active GameObject
        if (attackColliderInternal != null) attackColliderInternal.enabled = true; 

        // Reset Animator
        if (animator != null)
        {
            animator.ResetTrigger("DIE");
            animator.ResetTrigger("DAMAGE");
            animator.Play("Idle", 0, 0f); // Force back to Idle state
        }

        ApplyCurrentStats(); 
        Debug.Log($"Hunter {gameObject.name} Enabled/Respawned.");
    }

    private void Start() { /* Now handled by Awake and OnEnable */ }

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
        if (isDead) return; 

        mainHP -= damageAmount;

        if (mainHP <= 0)
        {
            isDead = true; 

            if (animator != null)
            {
                animator.SetTrigger("DIE");
            }
            else
            {
                Debug.LogError("Hunter: Animator component not found!", this);
            }

            // Disable components 
            if (capCollider != null) capCollider.enabled = false;
            if (navAgent != null) navAgent.enabled = false;

            // FIX: Explicitly disable the attack hand GameObject on death
            if (attackHandGO != null) attackHandGO.SetActive(false);

            // Disable the collider component
            if (attackColliderInternal != null) attackColliderInternal.enabled = false;

            StartCoroutine(DeathSequence());
        }
        else
        {
            if (animator != null) animator.SetTrigger("DAMAGE");
        }
    }

    private IEnumerator DeathSequence()
    {
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

        PrepareRespawn(this);
        gameObject.SetActive(false);
    }

    private static void PrepareRespawn(Hunter sourceSettingsHunter)
    {
        if (isRespawning) return;
        deathCount++;

        if (sourceSettingsHunter == null)
        {
            Debug.LogError("CRITICAL: Source Hunter for settings is null!");
            return;
        }

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

    private void EnsureCoroutineRunner()
    {
        if (coroutineRunner == null)
        {
            GameObject runnerGO = GameObject.Find("HunterCoroutineRunner");
            if (runnerGO == null) runnerGO = new GameObject("HunterCoroutineRunner");
            DontDestroyOnLoad(runnerGO); 
            coroutineRunner = runnerGO.GetComponent<EmptyMonoBehaviour>() ?? runnerGO.AddComponent<EmptyMonoBehaviour>();
        }
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRadius); 

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, detectionRadius); 

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, stopChaseRadius); 
    }
}

public class EmptyMonoBehaviour : MonoBehaviour { }