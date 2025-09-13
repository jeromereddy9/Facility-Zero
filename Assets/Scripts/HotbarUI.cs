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
    public float moveSpeed = 15f;
    public float sizeLerpSpeed = 15f;

    private RectTransform[] slotTransforms;
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
            playerInventory = FindFirstObjectByType<PlayerInventory>();

        // Store references to all slot transforms
        slotTransforms = new RectTransform[slotIcons.Length];
        for (int i = 0; i < slotIcons.Length; i++)
        {
            slotTransforms[i] = slotIcons[i].GetComponent<RectTransform>();
        }

        selectionHighlight.gameObject.SetActive(false);
        UpdateUI();
    }

    void Update()
    {
        // Smoothly move and resize the highlight
        if (selectionHighlight.gameObject.activeSelf)
        {
            // Smooth position movement
            selectionHighlight.rectTransform.position = Vector3.Lerp(
                selectionHighlight.rectTransform.position,
                new Vector3(targetPosition.x, targetPosition.y, selectionHighlight.rectTransform.position.z),
                Time.deltaTime * moveSpeed
            );

            // Smooth size transition
            selectionHighlight.rectTransform.sizeDelta = Vector2.Lerp(
                selectionHighlight.rectTransform.sizeDelta,
                targetSize,
                Time.deltaTime * sizeLerpSpeed
            );
        }
    }

    void UpdateUI()
    {
        if (playerInventory == null)
        {
            Debug.LogWarning("HotbarUI: No PlayerInventory reference!");
            return;
        }

        Debug.Log("Updating UI. Selected slot: " + playerInventory.selectedSlotIndex);

        // Handle selection highlight
        if (playerInventory.selectedSlotIndex >= 0 &&
            playerInventory.selectedSlotIndex < slotTransforms.Length &&
            slotTransforms[playerInventory.selectedSlotIndex] != null)
        {
            selectionHighlight.gameObject.SetActive(true);

            // Get the center position of the target slot in world space
            RectTransform targetSlot = slotTransforms[playerInventory.selectedSlotIndex];
            targetPosition = targetSlot.position;

            // Use the slot's size plus a small margin for the highlight
            targetSize = targetSlot.sizeDelta + new Vector2(20f, 20f);

            Debug.Log("Highlight target: Slot " + playerInventory.selectedSlotIndex + " at position " + targetPosition);
        }
        else
        {
            selectionHighlight.gameObject.SetActive(false);
            Debug.Log("Hiding highlight - no valid selection");
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

                Debug.Log("Slot " + i + ": " + item.itemName + " x" + item.quantity);
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

    // Debug method to test highlight positioning
    public void TestHighlightPosition(int slotIndex)
    {
        if (slotIndex >= 0 && slotIndex < slotTransforms.Length && slotTransforms[slotIndex] != null)
        {
            selectionHighlight.gameObject.SetActive(true);
            targetPosition = slotTransforms[slotIndex].position;
            targetSize = slotTransforms[slotIndex].sizeDelta + new Vector2(20f, 20f);

            // Snap immediately for testing
            selectionHighlight.rectTransform.position = targetPosition;
            selectionHighlight.rectTransform.sizeDelta = targetSize;

            Debug.Log("Test highlight at slot " + slotIndex + ", position: " + targetPosition);
        }
    }
}