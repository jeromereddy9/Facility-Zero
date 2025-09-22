using UnityEngine;
using FacilityZero.Manager;
using TMPro; // Add this for TextMeshPro

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
        [SerializeField] public int pelletDamage = 2;
        private float nextFireTime = 0f;

        [Header("Effect Lifetimes")]
        [SerializeField] private float effectLifetime = 0.5f;

        [Header("Ammo Settings")]
        [SerializeField] private int maxAmmo = 10;
        private int currentAmmo;

        [Header("UI")]
        [SerializeField] private TMP_Text ammoText;

        private InputManager inputManager;

        private void Start()
        {
            inputManager = GetComponentInParent<InputManager>();
            if (inputManager == null)
            {
                inputManager = FindObjectOfType<InputManager>(); // fallback
                if (inputManager == null)
                    Debug.LogError("Shooter: No InputManager found in parents or scene!");
            }

            currentAmmo = maxAmmo;
            UpdateAmmoUI();
        }

        private void Update()
        {
            if (inputManager != null && inputManager.Shoot && Time.time >= nextFireTime)
            {
                Shoot();
                nextFireTime = Time.time + fireRate;
            }
        }

        private void Shoot()
        {
            if (currentAmmo <= 0)
            {
                Debug.Log("No ammo!");
                return; // cannot shoot
            }

            currentAmmo--;
            UpdateAmmoUI();

            // --- MUZZLE FLASH ---
            if (Fire != null && FirePoint != null)
            {
                GameObject muzzleFlash = Instantiate(Fire, FirePoint);
                muzzleFlash.transform.localPosition = Vector3.zero;
                muzzleFlash.transform.localRotation = Quaternion.identity;

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
                    if (hit.collider.CompareTag("Enemy"))
                    {
                        var health = hit.collider.GetComponent<Enemy>();
                        if (health != null)
                        {
                            health.TakeDamage(pelletDamage);
                        }
                    }

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

        private void UpdateAmmoUI()
        {
            if (ammoText != null)
            {
                ammoText.text = currentAmmo + " / " + maxAmmo;

                // Low ammo warning: red when <= 25% of maxAmmo
                if (currentAmmo <= maxAmmo * 0.25f)
                    ammoText.color = Color.red;
                else
                    ammoText.color = Color.white;
            }
        }


        // Optional reload method
        public void Reload()
        {
            currentAmmo = maxAmmo;
            UpdateAmmoUI();
        }
    }
}
