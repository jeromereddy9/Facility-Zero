using UnityEngine;
using System.Collections;

public class ObjectPickUp : MonoBehaviour
{
    [Header("Pickup Settings")]
    public string itemName = "Item";
    public Sprite icon; // This will receive the generated sprite
    public GameObject itemPrefab;
    public int quantity = 1;

    [Header("Fallback Settings")]
    public Sprite fallbackIcon; // Assign a default icon in Inspector as backup
    public bool useFallbackIfNoIcon = true;

    private void Start()
    {
        // Start coroutine to check if icon was assigned
        StartCoroutine(VerifyIconAssignment());
    }

    private IEnumerator VerifyIconAssignment()
    {
        // Wait a moment for AutoIconGenerator to finish
        yield return new WaitForSeconds(0.5f);

        // If no icon was generated, use fallback
        if (icon == null && useFallbackIfNoIcon && fallbackIcon != null)
        {
            icon = fallbackIcon;
            Debug.Log($"Using fallback icon for: {itemName}");
        }

        if (icon == null)
        {
            Debug.LogWarning($"No icon assigned for: {itemName}. Hotbar will show empty slot.");
        }
        else
        {
            Debug.Log($"Icon ready for pickup: {itemName} (Sprite: {icon.name})");
        }
    }

    public void PickUp(PlayerInventory inventory)
    {
        if (inventory == null)
        {
            Debug.LogError("No inventory provided!");
            return;
        }

        // Ensure we have an icon (use fallback if needed)
        Sprite iconToUse = icon;
        if (iconToUse == null && fallbackIcon != null)
        {
            iconToUse = fallbackIcon;
        }

        bool added = inventory.AddItem(itemName, iconToUse, itemPrefab, quantity);
        if (added)
        {
            Debug.Log($"Picked up: {itemName} (Icon: {iconToUse != null})");
            Destroy(gameObject);
        }
    }

    // Editor method to manually assign icon
#if UNITY_EDITOR
    [ContextMenu("Assign Test Icon")]
    private void AssignTestIcon()
    {
        if (icon == null && fallbackIcon != null)
        {
            icon = fallbackIcon;
            UnityEditor.EditorUtility.SetDirty(this);
        }
    }
#endif
}