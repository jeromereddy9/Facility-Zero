using UnityEngine;
using FacilityZero.Manager;
using TMPro;
using System.Collections;

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
        [SerializeField] private int magCapacity = 7;     // bullets in magazine
        [SerializeField] public int totalAmmo = 56;      // bullets carried
        private int currentMag;

        [Header("UI")]
        [SerializeField] private TMP_Text ammoText;

        private InputManager inputManager;
        private bool isFlashing = false;

        private void Start()
        {
            inputManager = GetComponentInParent<InputManager>();
            if (inputManager == null)
            {
                inputManager = FindObjectOfType<InputManager>();
                if (inputManager == null)
                    Debug.LogError("Shooter: No InputManager found in parents or scene!");
            }

            currentMag = magCapacity;
            UpdateAmmoUI();
        }

        private void Update()
        {
            if (inputManager != null)
            {
                // Shooting
                if (inputManager.Shoot && Time.time >= nextFireTime)
                {
                    Shoot();
                    nextFireTime = Time.time + fireRate;
                }

                // Reload
                if (inputManager.Reload)
                {
                    Reload();
                }
            }
        }

        private void Shoot()
        {
            if (currentMag <= 0)
            {
                Debug.Log("No ammo in magazine!");
                return;
            }

            currentMag--;
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
                            health.TakeDamage(pelletDamage);
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

        public void Reload()
        {
            if (currentMag >= magCapacity || totalAmmo <= 0)
                return; // Magazine full or no ammo left

            int bulletsNeeded = magCapacity - currentMag;

            if (totalAmmo >= bulletsNeeded)
            {
                currentMag += bulletsNeeded;
                totalAmmo -= bulletsNeeded;
            }
            else
            {
                currentMag += totalAmmo;
                totalAmmo = 0;
            }

            UpdateAmmoUI();
        }

        public void UpdateAmmoUI()
        {
            if (ammoText != null)
            {
                ammoText.text = currentMag + " / " + totalAmmo;

                // Low ammo warning
                if (currentMag <= magCapacity * 0.25f)
                {
                    if (!isFlashing)
                        StartCoroutine(FlashAmmoWarning());
                }
                else
                {
                    ammoText.color = Color.white;
                }
            }
        }

        private IEnumerator FlashAmmoWarning()
        {
            isFlashing = true;
            while (currentMag <= magCapacity * 0.25f && currentMag > 0)
            {
                ammoText.color = Color.red;
                yield return new WaitForSeconds(0.3f);
                ammoText.color = Color.white;
                yield return new WaitForSeconds(0.3f);
            }
            ammoText.color = Color.white;
            isFlashing = false;
        }
    }
}
