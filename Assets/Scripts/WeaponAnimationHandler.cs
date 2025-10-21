using FacilityZero.Manager;
using UnityEngine;

namespace FacilityZero.Combat
{
    [RequireComponent(typeof(WeaponManager))]
    public class WeaponAnimationHandler : MonoBehaviour
    {
        private WeaponManager weaponManager;
        private FPInputManager inputManager;

        private void Start()
        {
            weaponManager = GetComponent<WeaponManager>();
            inputManager = weaponManager.inputManager;
        }

        private void Update()
        {
            if (weaponManager == null || inputManager == null) return;

            HandleAnimations();
        }

        private void HandleAnimations()
        {
            var weaponAnimator = weaponManager.GetCurrentWeaponAnimator();
            var armsAnimator = weaponManager.GetCurrentArmsAnimator();
            var shooter = weaponManager.GetCurrentShooter();

            if (shooter == null) return;

            // --- Shoot ---
            if (inputManager.ShootPressedThisFrame && shooter.CanShoot())
            {
                weaponAnimator?.ResetTrigger("Shoot");
                armsAnimator?.ResetTrigger("Shoot");

                weaponAnimator?.SetTrigger("Shoot");
                armsAnimator?.SetTrigger("Shoot");

                shooter.TryShoot();
            }

            // --- Reload ---
            if (inputManager.ReloadPressedThisFrame && shooter.TotalAmmo > 0 && shooter.CurrentMag < shooter.MagCapacity)
            {
                weaponAnimator?.ResetTrigger("Reload");
                armsAnimator?.ResetTrigger("Reload");

                weaponAnimator?.SetTrigger("Reload");
                armsAnimator?.SetTrigger("Reload");

                shooter.Reload();
            }
        }
    }
}
