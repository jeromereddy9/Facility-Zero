using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace FacilityZero.PlayerInventory
{
    [System.Serializable]
    public class InventoryItem
    {
        public Sprite icon;      // Icon for hotbar display
        public string tagName;   // Stores the prefab's tag
        public int quantity;

        public InventoryItem(Sprite iconSprite, string prefabTag, int initialQuantity)
        {
            icon = iconSprite;
            tagName = prefabTag;
            quantity = initialQuantity;
        }
    }

    public class PlayerInventory : MonoBehaviour,ISavable
    {
        public delegate void OnInventoryChanged();
        public static event OnInventoryChanged onInventoryChanged;

        public List<InventoryItem> items = new List<InventoryItem>();
        public int selectedSlotIndex = -1; // Currently selected hotbar slot

        void Start()
        {
            InitializeInventory();
        }

        void Update()
        {
            HandleHotbarSelection();
        }

        // Initialize empty inventory slots
        void InitializeInventory()
        {
            items.Clear();
            for (int i = 0; i < 3; i++) // 3-slot hotbar
            {
                items.Add(new InventoryItem(null, "", 0));
            }

            selectedSlotIndex = -1;
            onInventoryChanged?.Invoke();
            Debug.Log("Inventory initialized with " + items.Count + " empty slots");
        }

        // Handle slot selection via keyboard/mouse scroll
        void HandleHotbarSelection()
        {
            Keyboard keyboard = Keyboard.current;
            if (keyboard != null)
            {
                if (keyboard.digit1Key.wasPressedThisFrame) SelectSlot(0);
                if (keyboard.digit2Key.wasPressedThisFrame) SelectSlot(1);
                if (keyboard.digit3Key.wasPressedThisFrame) SelectSlot(2);
            }

            Mouse mouse = Mouse.current;
            if (mouse != null)
            {
                float scroll = mouse.scroll.ReadValue().y;
                if (scroll != 0 && items.Count > 0)
                {
                    int direction = scroll > 0 ? 1 : -1;
                    int newIndex = selectedSlotIndex == -1 ? 0 : selectedSlotIndex - direction;

                    if (newIndex < 0) newIndex = items.Count - 1;
                    if (newIndex >= items.Count) newIndex = 0;

                    SelectSlot(newIndex);
                }
            }
        }

        void SelectSlot(int newIndex)
        {
            if (newIndex < 0 || newIndex >= items.Count) return;

            selectedSlotIndex = (newIndex == selectedSlotIndex) ? -1 : newIndex;
            Debug.Log("Selected slot: " + selectedSlotIndex);
            onInventoryChanged?.Invoke();
        }

        // Add an item to inventory by stacking same tags
        public bool AddItem(Sprite icon, string tagName, int quantityToAdd = 1)
        {
            Debug.Log("Adding item with tag: " + tagName);

            // Stack with existing same-tag items
            foreach (var item in items)
            {
                if (item.tagName == tagName)
                {
                    item.quantity += quantityToAdd;
                    onInventoryChanged?.Invoke();
                    return true;
                }
            }

            // Find first empty slot
            foreach (var item in items)
            {
                if (item.tagName == "")
                {
                    item.icon = icon;
                    item.tagName = tagName;
                    item.quantity = quantityToAdd;
                    onInventoryChanged?.Invoke();
                    return true;
                }
            }

            Debug.LogWarning("Inventory full! Could not add: " + tagName);
            return false;
        }
        // can use this to decrement an item when called
        public void UseItem(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= items.Count) return;

            InventoryItem item = items[slotIndex];
            if (item.tagName != "" && item.quantity > 0)
            {
                item.quantity--;

                if (item.quantity <= 0)
                {
                    // Clear slot if quantity is zero
                    item.icon = null;
                    item.tagName = "";
                    item.quantity = 0;
                }

                onInventoryChanged?.Invoke();
            }
        }

        // can use this method to check if a tag is in the inventory
        // for example: bool hasKeycard = inventory.HasItemWithTag("Keycard");
        public bool HasItemWithTag(string tagName)
        {
            foreach (var item in items)
            {
                if (item.tagName == tagName && item.quantity > 0)
                {
                    return true;
                }
            }
            return false;
        }

        // Get the currently selected item
        // for example: InventoryItem selected = inventory.GetSelectedItem();
        public InventoryItem GetSelectedItem()
        {
            if (selectedSlotIndex >= 0 && selectedSlotIndex < items.Count)
                return items[selectedSlotIndex];
            return null;
        }

        public void SaveData(GameSaveData saveData)
        {
            // Clear previous saved inventory
            saveData.inventoryItems.Clear();

            // Save every slot, including empty ones
            foreach (var item in items)
            {
                saveData.inventoryItems.Add(new GameSaveData.InventoryItemData
                {
                    tagName = item.tagName,   // will be "" if empty
                    quantity = item.quantity
                });
            }

            // Save the selected hotbar slot
            saveData.selectedHotbarSlot = selectedSlotIndex;
        }

        public void LoadData(GameSaveData saveData)
        {
            // Ensure inventory has the correct number of slots
            int slots = 3;
            items.Clear();
            for (int i = 0; i < slots; i++)
            {
                items.Add(new InventoryItem(null, "", 0));
            }

            // Load saved data
            for (int i = 0; i < saveData.inventoryItems.Count && i < items.Count; i++)
            {
                var savedItem = saveData.inventoryItems[i];
                var slot = items[i];

                slot.tagName = savedItem.tagName;
                slot.quantity = savedItem.quantity;

                // Optional: Resolve icon from tag if needed
                // slot.icon = YourIconManager.GetIconByTag(savedItem.tagName);
            }

            // Restore selected slot
            selectedSlotIndex = saveData.selectedHotbarSlot;

            // Notify listeners (UI update)
            onInventoryChanged?.Invoke();
        }
    }
}
