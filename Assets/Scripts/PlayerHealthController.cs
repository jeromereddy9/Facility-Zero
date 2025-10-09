using FacilityZero.DeathScreen;
using FacilityZero.PlayerControl;
using FacilityZero.PlayerInventory;
using FacilityZero.Manager;
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

        private PlayerInventory.PlayerInventory playerInventory;
        private InputManager inputManager;

        private void Start()
        {
            HP = maxHP;

            if (healthBar != null)
            {
                healthBar.maxValue = maxHP;
                healthBar.value = HP;
            }

            playerInventory = GetComponent<PlayerInventory.PlayerInventory>();
            inputManager = FindObjectOfType<InputManager>();
        }

        private void Update()
        {
            if (isDead) return;
            if (inputManager == null) return;

            // Check for Med Kit usage
            if (inputManager.UseItem)
            {
                Debug.Log("H pressed");
                UseSelectedMedKit();
            }
        }

        private void UseSelectedMedKit()
        {
            if (playerInventory == null) return;
            // Search all slots for a Med Kit
            for (int i = 0; i < playerInventory.items.Count; i++)
            {
                var item = playerInventory.items[i];
                if (item.tagName == "Med Kit" && item.quantity > 0)
                {
                    if(HP != maxHP)
                    {
                        Heal(50); // adjust heal amount
                        playerInventory.UseItem(i);
                        Debug.Log("Used Med Kit from slot " + i + ". Healed 50 HP."+"Player health now, "+HP);
                        return;
                    }
                }
            }
        }


        public void TakeDamage(int damageAmount)
        {
            if (isDead) return;

            HP -= damageAmount;
            HP = Mathf.Clamp(HP, 0, maxHP);

            if (healthBar != null)
                healthBar.value = HP;

            if (HP <= maxHP * 0.25f)
                ActivateLowHealthWarning();

            if (HP <= 0)
                Die();
            else
                Debug.Log("Player Hit: " + HP + " HP remaining");
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

            // Trigger death screen
            var deathScreen = FindObjectOfType<DeathScreen.DeathScreen>();
            if (deathScreen != null)
                deathScreen.TriggerDeathScreen();
        }

        public void Heal(int healAmount)
        {
            HP += healAmount;
            HP = Mathf.Clamp(HP, 0, maxHP);

            if (healthBar != null)
                healthBar.value = HP;

            if (HP > maxHP * 0.25f)
                DisableLowHealthWarning();

            Debug.Log("Player healed: " + HP + " HP");
        }

        private void ActivateLowHealthWarning()
        {
            if (healthFill != null)
                healthFill.color = Color.red;
        }

        private void DisableLowHealthWarning()
        {
            if (healthFill != null)
                healthFill.color = Color.white;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("ZombieAttackHand"))
            {
                TakeDamage(15);
            }
        }
    }
}
