using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[System.Serializable]
public class InventoryItem
{
    public string itemName;
    public Sprite icon;
    public GameObject itemPrefab;
    public int quantity;

    public InventoryItem(string name, Sprite iconSprite, GameObject prefab, int initialQuantity)
    {
        itemName = name;
        icon = iconSprite;
        itemPrefab = prefab;
        quantity = initialQuantity;
    }
}

public class PlayerInventory : MonoBehaviour
{
    public delegate void OnInventoryChanged();
    public static event OnInventoryChanged onInventoryChanged;

    public List<InventoryItem> items = new List<InventoryItem>();
    public int selectedSlotIndex = -1; // Start with no selection

    void Start()
    {
        InitializeInventory();
    }

    void Update()
    {
        HandleHotbarSelection();
    }

    void InitializeInventory()
    {
        // Pre-populate 3 empty slots
        for (int i = 0; i < 3; i++)
        {
            items.Add(new InventoryItem("Empty", null, null, 0));
        }

        selectedSlotIndex = -1; // Ensure no slot is selected initially

        onInventoryChanged?.Invoke();
    }

    void HandleHotbarSelection()
    {
        Keyboard keyboard = Keyboard.current;
        if (keyboard == null) return;

        // Check number keys
        if (keyboard.digit1Key.wasPressedThisFrame) { SelectSlot(0); }
        if (keyboard.digit2Key.wasPressedThisFrame) { SelectSlot(1); }
        if (keyboard.digit3Key.wasPressedThisFrame) { SelectSlot(2); }

        // Optional: mouse scroll for changing slots
        Mouse mouse = Mouse.current;
        if (mouse != null)
        {
            float scroll = mouse.scroll.ReadValue().y;
            if (scroll != 0 && items.Count > 0)
            {
                int direction = scroll > 0 ? 1 : -1;
                int newIndex = selectedSlotIndex == -1 ? 0 : selectedSlotIndex - direction;

                // Wrap-around
                if (newIndex < 0) newIndex = items.Count - 1;
                if (newIndex >= items.Count) newIndex = 0;

                SelectSlot(newIndex);
            }
        }
    }

    void SelectSlot(int newIndex)
    {
        if (newIndex < 0 || newIndex >= items.Count) return;

        // Deselect if same slot pressed
        if (newIndex == selectedSlotIndex)
            selectedSlotIndex = -1;
        else
            selectedSlotIndex = newIndex;

        onInventoryChanged?.Invoke();
    }

    public bool AddItem(string itemName, Sprite icon, GameObject prefab, int quantityToAdd = 1)
    {
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].itemName == itemName)
            {
                items[i].quantity += quantityToAdd;
                onInventoryChanged?.Invoke();
                return true;
            }
        }

        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].itemName == "Empty")
            {
                items[i] = new InventoryItem(itemName, icon, prefab, quantityToAdd);
                onInventoryChanged?.Invoke();
                return true;
            }
        }

        Debug.Log("Inventory is full!");
        return false;
    }

    public void UseItem(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= items.Count) return;

        InventoryItem item = items[slotIndex];
        if (item.itemName != "Empty" && item.quantity > 0)
        {
            item.quantity--;

            if (item.itemPrefab != null)
            {
                Instantiate(item.itemPrefab, transform.position + transform.forward, transform.rotation);
            }

            if (item.quantity <= 0)
            {
                items[slotIndex] = new InventoryItem("Empty", null, null, 0);
            }

            onInventoryChanged?.Invoke();
        }
    }

    public InventoryItem GetSelectedItem()
    {
        if (selectedSlotIndex >= 0 && selectedSlotIndex < items.Count)
            return items[selectedSlotIndex];
        return null;
    }
}
