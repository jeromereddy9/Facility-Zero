using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HotbarUI : MonoBehaviour
{
    public PlayerInventory playerInventory;

    public Image[] slotIcons;
    public TextMeshProUGUI[] quantityTexts;

    public Image selectionHighlight;

    void OnEnable()
    {
        PlayerInventory.onInventoryChanged += UpdateUI;
    }

    void OnDisable()
    {
        PlayerInventory.onInventoryChanged -= UpdateUI;
    }

    void Start()
    {
        if (playerInventory == null)
            playerInventory = FindObjectOfType<PlayerInventory>();

        // Hide highlight at start - no selection initially
        selectionHighlight.gameObject.SetActive(false);

        // Initial UI update
        UpdateUI();
    }

    void UpdateUI()
    {
        if (playerInventory == null) return;

        // Handle selection highlight
        if (playerInventory.selectedSlotIndex >= 0 && playerInventory.selectedSlotIndex < slotIcons.Length)
        {
            selectionHighlight.gameObject.SetActive(true);

            // Automatically center highlight over the selected slot
            Image targetSlot = slotIcons[playerInventory.selectedSlotIndex];
            selectionHighlight.rectTransform.position = targetSlot.rectTransform.position;
            selectionHighlight.rectTransform.sizeDelta = targetSlot.rectTransform.sizeDelta + new Vector2(10, 10); // slightly bigger than slot
        }
        else
        {
            selectionHighlight.gameObject.SetActive(false);
        }

        // Update slot icons and quantity texts
        for (int i = 0; i < slotIcons.Length; i++)
        {
            if (i >= playerInventory.items.Count)
            {
                slotIcons[i].enabled = false;
                quantityTexts[i].text = "";
                continue;
            }

            InventoryItem item = playerInventory.items[i];

            if (item.itemName != "Empty" && item.quantity > 0)
            {
                slotIcons[i].sprite = item.icon;
                slotIcons[i].enabled = true;
                quantityTexts[i].text = item.quantity > 1 ? "x" + item.quantity.ToString() : "";
            }
            else
            {
                slotIcons[i].enabled = false;
                quantityTexts[i].text = "";
            }
        }
    }

    public void UseSelectedItem()
    {
        if (playerInventory != null && playerInventory.selectedSlotIndex >= 0)
        {
            playerInventory.UseItem(playerInventory.selectedSlotIndex);
        }
    }
}
