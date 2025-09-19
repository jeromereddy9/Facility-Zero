using UnityEngine;

public class ObjectPickUp : MonoBehaviour
{
    [Header("Pickup Settings")]
    public Sprite icon; // Assign a sprite manually or via AutoIconGenerator
    public GameObject itemPrefab;
    public int quantity = 1;

    // Use the GameObject tag for identification
    public string ItemType => gameObject.tag;

    public void PickUp(PlayerInventory inventory)
    {
        if (inventory == null)
        {
            Debug.LogError("No inventory provided!");
            return;
        }

        if (inventory.AddItem(ItemType, icon, itemPrefab, quantity))
        {
            Debug.Log($"Picked up: {ItemType} (Quantity: {quantity})");
            Destroy(gameObject);
        }
        else
        {
            Debug.LogWarning($"Inventory full! Could not pick up: {ItemType}");
        }
    }
}
