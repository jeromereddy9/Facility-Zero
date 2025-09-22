using UnityEngine;
using FacilityZero.PlayerInventory;
using FacilityZero.GunController;

public class ObjectPickUp : MonoBehaviour
{
    [Header("Pickup Settings")]
    [SerializeField] public Sprite icon;        // Icon for hotbar
    [SerializeField] public Sprite fallbackIcon; // Backup icon if none assigned
    [SerializeField] public string tagName;     
    [SerializeField] public int ammoToAdd = 7;  // amount added if ammo box

    [Header("References")]
    [SerializeField] private Shooter shooter;   

    private void Start()
    {
        
        if (icon == null && fallbackIcon != null)
            icon = fallbackIcon;
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
        Debug.Log("Picked up item with tag: " + gameObject.tag);

        
        if (tagName == "Ammo Box" && shooter != null)
        {
            // Add ammo directly
            shooter.totalAmmo += ammoToAdd;
            shooter.UpdateAmmoUI();

            // Remove the ammo box from inventory instantly (so it never stays there)
            int slotIndex = inventory.items.FindIndex(item => item.tagName == "Ammo Box");
            if (slotIndex >= 0)
                inventory.UseItem(slotIndex); // consumes item immediately

            Debug.Log("AmmoBox picked up and applied! +" + ammoToAdd + " ammo");
        }
    }
}
