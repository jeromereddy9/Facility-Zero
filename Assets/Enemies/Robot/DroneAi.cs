using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DroneAi : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private NavMeshAgent navAgent;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Transform firePoint;   // Firepoint 1
    [SerializeField] private Transform firePoint2;  // Firepoint 2
    [SerializeField] private GameObject projectilePrefab;

    [Header("Patrol Settings")]
    public Transform waypointCluster; // Assign in Inspector
    public float patrolSpeed = 5f;
    public float visionRange = 14f;
    public float engagementRange = 10f;

    private List<Transform> wayPointsList = new List<Transform>();
    private Vector3 currentPatrolPoint;
    private bool hasPatrolPoint;

    [Header("Combat Settings")]
    public float attackCoolDown = 1f;
    private bool isOnAttackCooldown;
    public float forwardShotForce = 10f;
    public float verticalShotForce = 5f;

    [SerializeField] private GameObject muzzleFlashPrefab; // assign in Inspector
    [SerializeField] private float muzzleFlashDuration = 0.3f; // how long it lasts

    private bool isPlayerVisible;
    private bool isPlayerInRange;

    // track which firepoint to use next
    private bool useFirstFirepoint = true;

    private void Awake()
    {
        // Auto-assign player if not set
        if (playerTransform == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null) playerTransform = player.transform;
        }

        // Auto-assign NavMeshAgent
        if (navAgent == null)
            navAgent = GetComponent<NavMeshAgent>();

        navAgent.speed = patrolSpeed;
        navAgent.stoppingDistance = 0.2f;

        // Populate waypoints from cluster
        if (waypointCluster != null)
        {
            wayPointsList.Clear();
            foreach (Transform t in waypointCluster)
                wayPointsList.Add(t);
        }

        // Pick initial patrol point
        PickNextPatrolPoint();
    }

    private void Update()
    {
        DetectPlayer();
        UpdateBehaviourState();
    }

    private void DetectPlayer()
    {
        if (playerTransform == null) return;

        float distance = Vector3.Distance(transform.position, playerTransform.position);
        isPlayerVisible = distance <= visionRange;
        isPlayerInRange = distance <= engagementRange;
    }

    private void FireProjectile()
    {
        if (projectilePrefab == null) return;

        // pick which firepoint to use this time
        Transform activeFirePoint = useFirstFirepoint ? firePoint : firePoint2;
        if (activeFirePoint == null) return;

        // --- Instantiate Projectile ---
        Rigidbody projectileRb = Instantiate(projectilePrefab, activeFirePoint.position, activeFirePoint.rotation).GetComponent<Rigidbody>();
        if (projectileRb != null)
        {
            projectileRb.useGravity = false;
            projectileRb.velocity = transform.forward * forwardShotForce;
        }
        Destroy(projectileRb.gameObject, 2f);

        // --- Instantiate Muzzle Flash ---
        if (muzzleFlashPrefab != null)
        {
            GameObject flash = Instantiate(muzzleFlashPrefab, activeFirePoint.position, activeFirePoint.rotation);
            flash.transform.parent = activeFirePoint; // attach to current firepoint
            Destroy(flash, muzzleFlashDuration);
        }

        // alternate firepoints for next shot
        useFirstFirepoint = !useFirstFirepoint;
    }

    private void PickNextPatrolPoint()
    {
        if (wayPointsList.Count == 0) return;

        currentPatrolPoint = wayPointsList[Random.Range(0, wayPointsList.Count)].position;
        navAgent.SetDestination(currentPatrolPoint);
        hasPatrolPoint = true;
    }

    private void PerformPatrol()
    {
        if (wayPointsList.Count == 0) return;

        // If reached the current patrol point, pick a new one
        if (navAgent.remainingDistance <= navAgent.stoppingDistance)
        {
            PickNextPatrolPoint();
        }
    }

    private void PerformChase()
    {
        if (playerTransform != null)
            navAgent.SetDestination(playerTransform.position);
    }

    private void PerformAttack()
    {
        // Stop moving while attacking
        navAgent.SetDestination(transform.position);

        // Face the player
        if (playerTransform != null)
            transform.LookAt(playerTransform);

        if (!isOnAttackCooldown)
        {
            FireProjectile();
            StartCoroutine(AttackCoolDownRoutine());
        }
    }

    private IEnumerator AttackCoolDownRoutine()
    {
        isOnAttackCooldown = true;
        yield return new WaitForSeconds(attackCoolDown);
        isOnAttackCooldown = false;
    }

    private void UpdateBehaviourState()
    {
        if (!isPlayerVisible && !isPlayerInRange)
        {
            PerformPatrol();
        }
        else if (isPlayerVisible && !isPlayerInRange)
        {
            PerformChase();
        }
        else if (isPlayerVisible && isPlayerInRange)
        {
            PerformAttack();
        }
    }
}
