using UnityEngine;

namespace FacilityZero.Combat
{
    [RequireComponent(typeof(WeaponManager))]
    public class WeaponAnimationHandler : MonoBehaviour
    {
        private WeaponManager weaponManager;
        private FacilityZero.Manager.FPInputManager inputManager;

        private void Start()
        {
            weaponManager = GetComponent<WeaponManager>();
            inputManager = weaponManager.inputManager;
        }

        private void Update()
        {
            if (weaponManager == null || inputManager == null)
                return;

            HandleAnimations();
        }

        private void HandleAnimations()
        {
            var weaponAnimator = weaponManager.GetCurrentWeaponAnimator();
            var armsAnimator = weaponManager.GetCurrentArmsAnimator();

            // --- Actions ---
            if (inputManager.ShootPressedThisFrame)
            {
                weaponAnimator?.ResetTrigger("Shoot"); // prevents overlap
                armsAnimator?.ResetTrigger("Shoot");
                weaponAnimator?.SetTrigger("Shoot");
                armsAnimator?.SetTrigger("Shoot");
            }

            if (inputManager.ReloadPressedThisFrame)
            {
                weaponAnimator?.ResetTrigger("Reload");
                armsAnimator?.ResetTrigger("Reload");
                weaponAnimator?.SetTrigger("Reload");
                armsAnimator?.SetTrigger("Reload");
            }
        }
    }
}
