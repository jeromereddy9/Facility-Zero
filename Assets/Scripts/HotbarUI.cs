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

    [Header("Smooth Movement Settings")]
    public float moveSpeed = 10f; // how fast the highlight moves
    public float sizeLerpSpeed = 10f; // how fast the highlight resizes

    private Vector2 targetPosition;
    private Vector2 targetSize;

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

        selectionHighlight.gameObject.SetActive(false);

        UpdateUI();
    }

    void Update()
    {
        // Smoothly move and resize highlight towards target
        if (selectionHighlight.gameObject.activeSelf)
        {
            selectionHighlight.rectTransform.position = Vector2.Lerp(
                selectionHighlight.rectTransform.position,
                targetPosition,
                Time.deltaTime * moveSpeed
            );

            selectionHighlight.rectTransform.sizeDelta = Vector2.Lerp(
                selectionHighlight.rectTransform.sizeDelta,
                targetSize,
                Time.deltaTime * sizeLerpSpeed
            );
        }
    }

    void UpdateUI()
    {
        if (playerInventory == null) return;

        // Handle highlight
        if (playerInventory.selectedSlotIndex >= 0 && playerInventory.selectedSlotIndex < slotIcons.Length)
        {
            selectionHighlight.gameObject.SetActive(true);

            Image targetSlot = slotIcons[playerInventory.selectedSlotIndex];
            targetPosition = targetSlot.rectTransform.position;
            targetSize = targetSlot.rectTransform.sizeDelta + new Vector2(10, 10);
        }
        else
        {
            selectionHighlight.gameObject.SetActive(false);
        }

        // Update slot icons and quantities
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
