using UnityEngine;
using FacilityZero.PlayerInventory;
using FacilityZero.GunController;
using FacilityZero.Combat;
using FacilityZero.PlayerHealthController; // for PlayerHealth

public class ObjectPickUp : MonoBehaviour
{
    [Header("Pickup Settings")]
    [SerializeField] public Sprite icon;
    [SerializeField] public Sprite fallbackIcon;
    [SerializeField] public string tagName;
    [SerializeField] public int ammoToAdd = 5;
    [SerializeField] public int hpIncreaseAmount = 100; // amount to increase max HP for Datapads

    private WeaponManager weaponManager;

    private void Start()
    {
        if (icon == null && fallbackIcon != null)
            icon = fallbackIcon;

        weaponManager = FindFirstObjectByType<WeaponManager>();
        if (weaponManager == null)
            weaponManager = FindObjectOfType<WeaponManager>(true);

        if (weaponManager == null)
            Debug.LogError("❌ No WeaponManager found in scene!");
        else
            Debug.Log($"✅ Found WeaponManager: {weaponManager.name}");
    }

    public void PickUp(PlayerInventory inventory)
    {
        if (inventory == null)
        {
            Debug.LogError("No inventory provided!");
            return;
        }

        Sprite iconToUse = icon != null ? icon : fallbackIcon;
        inventory.AddItem(iconToUse, gameObject.tag, 1);
        Destroy(gameObject);

        Debug.Log($"🧤 Picked up item: {gameObject.tag}");

        // Handle ammo pickup
        if (tagName == "Ammo Box" && weaponManager != null)
        {
            var activeShooter = weaponManager.GetCurrentShooter();
            if (activeShooter != null)
            {
                activeShooter.totalAmmo += ammoToAdd;
                activeShooter.UpdateAmmoUI();

                int slotIndex = inventory.items.FindIndex(item => item.tagName == "Ammo Box");
                if (slotIndex >= 0)
                    inventory.UseItem(slotIndex); // consume ammo box
            }
        }

        // Handle datapad pickup (increase max HP)
        if (tagName == "Datapad")
        {
            PlayerHealth playerHealth = FindObjectOfType<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.maxHP += hpIncreaseAmount;
                playerHealth.Heal(hpIncreaseAmount);
                int slotIndex = inventory.items.FindIndex(item => item.tagName == "Datapad");
                if (slotIndex >= 0)
                    inventory.UseItem(slotIndex); 
                Debug.Log($"Datapad picked up! Max HP increased by {hpIncreaseAmount}. New Max HP: {playerHealth.maxHP}");
            }
            else
            {
                Debug.LogWarning("No PlayerHealth found in scene to apply Datapad bonus!");
            }
        }
    }
}
