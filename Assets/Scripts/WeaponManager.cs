using UnityEngine;
using FacilityZero.Manager;
using FacilityZero.GunController;

namespace FacilityZero.Combat
{
    [RequireComponent(typeof(WeaponAnimationHandler))]
    public class WeaponManager : MonoBehaviour
    {
        [Header("Weapons")]
        public GameObject[] weapons;              // world weapon models
        public Animator[] armsAnimators;          // FPS arms for each weapon
        public Shooter[] shooters;                // shooter scripts for each weapon

        [Header("Input")]
        public FPInputManager inputManager;

        private int currentWeaponIndex = 0;

        private void Start()
        {
            if (inputManager == null)
                inputManager = FindObjectOfType<FPInputManager>();

            EquipWeapon(currentWeaponIndex);
        }

        private void Update()
        {
            if (inputManager == null) return;

            if (inputManager.CycleWeapons)
            {
                CycleNextWeapon();
                inputManager.CycleWeapons = false;
            }
        }

        private void CycleNextWeapon()
        {
            int nextIndex = currentWeaponIndex + 1;
            if (nextIndex >= weapons.Length) nextIndex = 0;
            EquipWeapon(nextIndex);
        }

        private void EquipWeapon(int index)
        {
            if (index < 0 || index >= weapons.Length) return;

            // Hide all weapons & arms first
            for (int i = 0; i < weapons.Length; i++)
            {
                if (weapons[i] != null) weapons[i].SetActive(false);
                if (armsAnimators[i] != null)
                {
                    var renderers = armsAnimators[i].GetComponentsInChildren<SkinnedMeshRenderer>();
                    foreach (var r in renderers) r.enabled = false;
                }
            }

            currentWeaponIndex = index;

            // Show selected weapon
            if (weapons[index] != null) weapons[index].SetActive(true);
            if (armsAnimators[index] != null)
            {
                var renderers = armsAnimators[index].GetComponentsInChildren<SkinnedMeshRenderer>();
                foreach (var r in renderers) r.enabled = true;

                armsAnimators[index].SetTrigger("Equip");
            }

            // Trigger world weapon equip animation
            var anim = GetCurrentWeaponAnimator();
            if (anim != null)
                anim.SetTrigger("Equip");
        }

        public Animator GetCurrentWeaponAnimator()
        {
            if (weapons.Length == 0 || currentWeaponIndex >= weapons.Length) return null;
            return weapons[currentWeaponIndex]?.GetComponent<Animator>();
        }

        public Animator GetCurrentArmsAnimator()
        {
            if (armsAnimators.Length == 0 || currentWeaponIndex >= armsAnimators.Length) return null;
            return armsAnimators[currentWeaponIndex];
        }

        public Shooter GetCurrentShooter()
        {
            if (shooters.Length == 0 || currentWeaponIndex >= shooters.Length) return null;
            return shooters[currentWeaponIndex];
        }
    }
}

