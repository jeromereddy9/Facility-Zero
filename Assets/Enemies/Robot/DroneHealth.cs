using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class DroneHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHP = 50;   // Maximum health
    private int currentHP;    // Current health

    [Header("Death Effects")]
    [SerializeField] private GameObject explosionPrefab;  // Assign your explosion prefab
    [SerializeField] private float explosionDuration = 2f; // How long the explosion lasts

    private void Awake()
    {
        currentHP = maxHP;    // Initialize health
    }

    public void TakeDamage(int damageAmount)
    {
        currentHP -= damageAmount;

        if (currentHP <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        // Optional: disable components like NavMeshAgent, Collider, etc.
        NavMeshAgent navAgent = GetComponent<NavMeshAgent>();
        if (navAgent != null)
            navAgent.enabled = false;

        Collider col = GetComponent<Collider>();
        if (col != null)
            col.enabled = false;

        // --- Instantiate Explosion ---
        if (explosionPrefab != null)
        {
            GameObject explosion = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            Destroy(explosion, explosionDuration); // Destroy the explosion after its duration
        }

        // Destroy the drone immediately
        Destroy(gameObject);
    }
}



