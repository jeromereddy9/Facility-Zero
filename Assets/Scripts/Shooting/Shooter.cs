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
        [SerializeField] private float fireRate = 0.5f;
        [SerializeField] public int pelletDamage = 2;
        private float nextFireTime = 0f;

        [Header("Ammo Settings")]
        [SerializeField] public int magCapacity = 7;
        [SerializeField] public int totalAmmo = 56;
        public int currentMag;

        [Header("UI")]
        [SerializeField] private TMP_Text ammoText;

        [Header("Audio")]
        [SerializeField] private AudioSource shootAudioSource; //
        [SerializeField] private AudioClip shootClip;          

        private bool isFlashing = false;

       
        public int CurrentMag => currentMag;
        public int MagCapacity => magCapacity;
        public int TotalAmmo => totalAmmo;

        private void Start()
        {
            currentMag = magCapacity;
            UpdateAmmoUI();
        }

        public bool CanShoot()
        {
            return currentMag > 0 && Time.time >= nextFireTime;
        }

        public void TryShoot()
        {
            if (!CanShoot()) return;

            Shoot();
            nextFireTime = Time.time + fireRate;
        }

        private void Shoot()
        {
            currentMag--;
            UpdateAmmoUI();

            // --- Play shoot sound ---
            if (shootAudioSource != null && shootClip != null)
            {
                shootAudioSource.PlayOneShot(shootClip);
            }

            // Muzzle flash
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

                Destroy(muzzleFlash, 0.5f);
            }

            // Pellets
            for (int i = 0; i < pelletCount; i++)
            {
                Vector3 dir = GetSpreadDirection();
                if (Physics.Raycast(FirePoint.position, dir, out RaycastHit hit, fireRange))
                {
                    if (hit.collider.CompareTag("Enemy") && hit.collider.TryGetComponent<Enemy>(out var e)) e.TakeDamage(pelletDamage);
                    else if (hit.collider.CompareTag("Hunter") && hit.collider.TryGetComponent<Hunter>(out var h)) h.TakeDamage(pelletDamage);
                    else if (hit.collider.CompareTag("Robot") && hit.collider.TryGetComponent<DroneHealth>(out var d)) d.TakeDamage(pelletDamage);

                    if (HitPoint != null)
                    {
                        GameObject hitEffect = Instantiate(HitPoint, hit.point, Quaternion.LookRotation(hit.normal));
                        Destroy(hitEffect, 0.5f);
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
            if (currentMag >= magCapacity || totalAmmo <= 0) return;

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

                if (currentMag <= magCapacity * 0.25f)
                {
                    if (!isFlashing)
                        StartCoroutine(FlashAmmoWarning());
                }
                else ammoText.color = Color.white;
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
