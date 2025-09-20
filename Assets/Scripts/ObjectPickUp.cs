using UnityEngine;

public class ObjectPickUp : MonoBehaviour
{
    [Header("Pickup Settings")]
    public Sprite icon; // Icon for hotbar
    public Sprite fallbackIcon; // Backup icon if none assigned

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
        inventory.AddItem(iconToUse, gameObject.tag, 1); // Store icon and tag
        Destroy(gameObject);
        Debug.Log("Picked up item with tag: " + gameObject.tag);
    }
}
