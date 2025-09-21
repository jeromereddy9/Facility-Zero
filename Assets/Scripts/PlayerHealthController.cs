using FacilityZero.DeathScreen;
using FacilityZero.PlayerControl;
using UnityEngine;
using UnityEngine.UI;

namespace FacilityZero.PlayerHealthController
{
    public class PlayerHealth : MonoBehaviour
    {
        [Header("Player Stats")]
        public int maxHP = 300;
        public int HP;

        [Header("UI")]
        [SerializeField] private Slider healthBar;
        [SerializeField] private Image healthFill;

        private bool isDead = false;

        private void Start()
        {
            HP = maxHP;

            if (healthBar != null)
            {
                healthBar.maxValue = maxHP;
                healthBar.value = HP;
            }
        }

        public void TakeDamage(int damageAmount)
        {
            if (isDead) return;

            HP -= damageAmount;
            HP = Mathf.Clamp(HP, 0, maxHP);

            if (healthBar != null)
                healthBar.value = HP;

            if (HP <= maxHP * 0.25)
            {
                ActivateLowHealthWarning();
            }
            if (HP <= 0)
            {
                Die();
            }
            else
            {
                Debug.Log("Player Hit: " + HP + " HP remaining");
            }
        }

        private void Die()
        {
            if (isDead) return; 
            isDead = true;
            Debug.Log("Player Dead");

            // Disable player controls 
            var controller = GetComponent<PlayerController>();
            if (controller != null)
                controller.enabled = false;

            DeathScreen.DeathScreen deathScreen = FindObjectOfType<DeathScreen.DeathScreen>();
            if (deathScreen != null)
                deathScreen.TriggerDeathScreen();

        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("ZombieAttackHand"))
            {
                TakeDamage(15);
            }
        }


        public void Heal(int healAmount)
        {
            HP += healAmount;
            HP = Mathf.Clamp(HP, 0, maxHP);

            if (healthBar != null)
                healthBar.value = HP;

            if (HP > maxHP * 0.25)
            {
                DisableLowHealthWarning();
            }

            Debug.Log("Player healed: " + HP + " HP");
        }

        private void ActivateLowHealthWarning()
        {
            healthFill.color = Color.red;
        }
        private void DisableLowHealthWarning()
        {
            healthFill.color = Color.white;
        }

    }
}
