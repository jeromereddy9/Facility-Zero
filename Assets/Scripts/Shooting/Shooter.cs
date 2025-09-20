using UnityEngine;
using FacilityZero.Manager;

namespace FacilityZero.GunController
{
    public class Shooter : MonoBehaviour
    {
        [Header("References")]
        public Transform FirePoint;
        public GameObject Fire;
        public GameObject HitPoint;

        [Header("Shotgun Settings")]
        [SerializeField] private int pelletCount = 8;
        [SerializeField] private float spreadAngle = 8f;
        [SerializeField] private float fireRange = 20f;
        [SerializeField] private float fireRate = 0.5f; // seconds between shots
        private float nextFireTime = 0f;

        [Header("Effect Lifetimes")]
        [SerializeField] private float effectLifetime = 0.5f;

        private InputManager inputManager;

        private void Start()
        {
            inputManager = GetComponentInParent<InputManager>();
            if (inputManager == null)
            {
                inputManager = FindObjectOfType<InputManager>(); // fallback
                if (inputManager == null)
                {
                    Debug.LogError("Shooter: No InputManager found in parents or scene!");
                }
            }
        }

        private void Update()
        {
            // Only fire if enough time has passed since last shot
            if (inputManager != null && inputManager.Shoot && Time.time >= nextFireTime)
            {
                Shoot();
                nextFireTime = Time.time + fireRate;
            }
        }

        private void Shoot()
        {
            // --- MUZZLE FLASH ---
            if (Fire != null && FirePoint != null)
            {
                // Parent to FirePoint so it always sticks to the barrel
                GameObject muzzleFlash = Instantiate(Fire, FirePoint);
                muzzleFlash.transform.localPosition = Vector3.zero;
                muzzleFlash.transform.localRotation = Quaternion.identity;

                // Play particle system if present
                var ps = muzzleFlash.GetComponent<ParticleSystem>();
                if (ps != null)
                {
                    ps.Clear();
                    ps.Play();
                }

                Destroy(muzzleFlash, effectLifetime);
            }

            // --- PELLETS / HITS ---
            for (int i = 0; i < pelletCount; i++)
            {
                Vector3 dir = GetSpreadDirection();
                if (Physics.Raycast(FirePoint.position, dir, out RaycastHit hit, fireRange))
                {
                    if (HitPoint != null)
                    {
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
}
