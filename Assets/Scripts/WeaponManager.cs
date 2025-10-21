using UnityEngine;

namespace FacilityZero.Combat
{
    [RequireComponent(typeof(WeaponAnimationHandler))]
    public class WeaponManager : MonoBehaviour
    {
        [Header("Weapons")]
        public GameObject pistol;
        public GameObject shotgun;

        [Header("Arms")]
        public Animator armsPistolAnimator;
        public Animator armsShotgunAnimator;

        [Header("Input")]
        public FacilityZero.Manager.FPInputManager inputManager;

        private GameObject[] weapons;
        private Animator[] arms;
        private Animator currentWeaponAnimator;
        private Animator currentArmsAnimator;
        private int currentWeaponIndex = -1;

        private void Start()
        {
            weapons = new GameObject[] { pistol, shotgun };
            arms = new Animator[] { armsPistolAnimator, armsShotgunAnimator };

            if (inputManager == null)
                inputManager = FindObjectOfType<FacilityZero.Manager.FPInputManager>();

            SetUnarmedState();
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
            currentWeaponIndex++;

            if (currentWeaponIndex >= weapons.Length)
                currentWeaponIndex = 0; // loop back to start

            EquipWeapon(currentWeaponIndex);
        }

        private void SetUnarmedState()
        {
            // Disable all weapons and arms
            foreach (var w in weapons)
                if (w != null) w.SetActive(false);

            foreach (var a in arms)
                if (a != null) a.gameObject.SetActive(false);

            currentWeaponAnimator = null;
            currentArmsAnimator = null;
        }

        private void EquipWeapon(int index)
        {
            SetUnarmedState();

            if (index < 0 || index >= weapons.Length) return;

            // Enable chosen weapon
            weapons[index].SetActive(true);
            currentWeaponAnimator = weapons[index].GetComponent<Animator>();
            currentWeaponAnimator?.SetTrigger("Equip");

            // Enable matching arms
            if (arms[index] != null)
            {
                arms[index].gameObject.SetActive(true);
                currentArmsAnimator = arms[index];
                currentArmsAnimator.SetTrigger("Equip");
            }
        }

        public Animator GetCurrentWeaponAnimator() => currentWeaponAnimator;
        public Animator GetCurrentArmsAnimator() => currentArmsAnimator;
    }
}
