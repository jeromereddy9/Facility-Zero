using System.Collections;
using UnityEngine;
using UnityEngine.UI;
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

        // Damage (hit) flash 
        private Coroutine flashRoutine;

        // Low health warning
        private bool isLowHealth => HP <= maxHP * 0.25f; 
        private float pulseSpeed = 5f;                    
        private float pulseAmount = 0.5f;      
        
        private void Start()

        {
            HP = maxHP;

            if (healthBar != null)
            {
                healthBar.maxValue = maxHP;
                healthBar.value = HP;
            }

            UpdateHealthColour();

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

            if (healthFill != null)
            {
                if (isLowHealth) // trigger pulse effect
                {
                    float alphaPulse = 0.5f + Mathf.Sin(Time.time * pulseSpeed) * 0.25f; // 0.25 amplitude
                    Color c = healthFill.color;
                    healthFill.color = new Color(c.r, c.g, c.b, alphaPulse);
                }
                else
                {
                    // Reset scale when HP > 25%
                    healthFill.rectTransform.localScale = Vector3.one;
                }
            }
        }

        private void UpdateHealthColour()
        {
            if (healthFill == null) return;

            float healthPercent = (float)HP / maxHP;

            // Smoothly blend from Red (low) → Yellow (medium) → Green (full)
            if (healthPercent > 0.5f)
                healthFill.color = Color.Lerp(Color.yellow, Color.green, (healthPercent - 0.5f) * 2f);
            else
                healthFill.color = Color.Lerp(Color.red, Color.yellow, healthPercent * 2f);
        }

        private IEnumerator FlashDamage()
        {
            if (healthFill == null) yield break;

            Color originalColor = healthFill.color;
            healthFill.color = Color.white; // flash bright white or red for impact
            yield return new WaitForSeconds(0.1f); // short flash duration
            healthFill.color = originalColor;      // restore current gradient color
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
                        UpdateHealthColour(); // update health colour after healing
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

            UpdateHealthColour();

            if (flashRoutine != null)
                StopCoroutine(flashRoutine);
            flashRoutine = StartCoroutine(FlashDamage());

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

            UpdateHealthColour();

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
            else if (other.CompareTag("HunterAttackHand"))
            {
                TakeDamage(15); // hunter damage
            }
            else if (other.CompareTag("DroneProjectile"))
            {
                TakeDamage(3);
            }
        }
    }
}
