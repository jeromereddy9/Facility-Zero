using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[System.Serializable]
public class PlayerInventory : MonoBehaviour
{
    public delegate void OnInventoryChanged();
    public static event OnInventoryChanged onInventoryChanged;

    [System.Serializable]
    public class InventoryItem
    {
        public string itemType; // Uses GameObject tag
        public Sprite icon;
        public GameObject prefab;
        public int quantity;

        public InventoryItem(string type, Sprite iconSprite, GameObject prefabObj, int amount)
        {
            itemType = type;
            icon = iconSprite;
            prefab = prefabObj;
            quantity = amount;
        }
    }

    public List<InventoryItem> items = new List<InventoryItem>();
    public int selectedSlotIndex = -1;

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
        items.Clear();
        for (int i = 0; i < 3; i++)
            items.Add(new InventoryItem("Empty", null, null, 0));

        selectedSlotIndex = -1;
        onInventoryChanged?.Invoke();
    }

    void HandleHotbarSelection()
    {
        Keyboard keyboard = Keyboard.current;
        if (keyboard == null) return;

        if (keyboard.digit1Key.wasPressedThisFrame) SelectSlot(0);
        if (keyboard.digit2Key.wasPressedThisFrame) SelectSlot(1);
        if (keyboard.digit3Key.wasPressedThisFrame) SelectSlot(2);

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

    void SelectSlot(int index)
    {
        if (index < 0 || index >= items.Count) return;

        selectedSlotIndex = index == selectedSlotIndex ? -1 : index;
        onInventoryChanged?.Invoke();
    }

    public bool AddItem(string tagName, Sprite icon, GameObject prefab, int quantityToAdd = 1)
    {
        // Stack with existing item
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].itemType == tagName)
            {
                items[i].quantity += quantityToAdd;
                onInventoryChanged?.Invoke();
                return true;
            }
        }

        // Add to empty slot
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].itemType == "Empty")
            {
                items[i] = new InventoryItem(tagName, icon, prefab, quantityToAdd);
                onInventoryChanged?.Invoke();
                return true;
            }
        }

        return false;
    }

    public void UseItem(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= items.Count) return;

        InventoryItem item = items[slotIndex];
        if (item.itemType != "Empty" && item.quantity > 0)
        {
            item.quantity--;

            if (item.prefab != null)
                Instantiate(item.prefab, transform.position + transform.forward, transform.rotation);

            if (item.quantity <= 0)
                items[slotIndex] = new InventoryItem("Empty", null, null, 0);

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
