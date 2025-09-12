using UnityEngine;
using System.Collections;

public class Shooter : MonoBehaviour
{
    [Header("References")]
    public Transform FirePoint;
    public GameObject Fire;
    public GameObject HitPoint;

    [Header("Shotgun Settings")]
    public int pelletCount = 8;
    public float spreadAngle = 8f;
    public float fireRange = 20f;
    public float fireRate = 0.5f; // A value of 0.5f means 2 seconds between shots

    private float nextFireTime = 0f;

    [Header("Effect Lifetimes")]
    public float effectLifetime = 0.5f;

    // Use Update to trigger the automatic firing
    private void Update()
    {
        // Fire automatically based on the fire rate
        if (Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + 1f / fireRate;
        }
    }

    private void Shoot()
    {
        // Spawn muzzle effect and destroy it after a short time
        if (Fire != null)
        {
            GameObject muzzleFlash = Instantiate(Fire, FirePoint.position, FirePoint.rotation);
            Destroy(muzzleFlash, effectLifetime);
        }

        // Fire multiple pellets
        for (int i = 0; i < pelletCount; i++)
        {
            Vector3 dir = GetSpreadDirection();

            if (Physics.Raycast(FirePoint.position, dir, out RaycastHit hit, fireRange))
            {
                if (HitPoint != null)
                {
                    // Spawn hit effect and destroy it after a short time
                    GameObject hitEffect = Instantiate(HitPoint, hit.point, Quaternion.LookRotation(hit.normal));
                    Destroy(hitEffect, effectLifetime);
                }
            }
        }
    }

    private Vector3 GetSpreadDirection()
    {
        float yaw = Random.Range(-spreadAngle, spreadAngle);
        float pitch = Random.Range(-spreadAngle, spreadAngle);

        Quaternion spreadRot = Quaternion.Euler(pitch, yaw, 0);
        return spreadRot * FirePoint.forward;
    }
}
