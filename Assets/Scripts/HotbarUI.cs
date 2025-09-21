using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using FacilityZero.PlayerInventory;

public class HotbarUI : MonoBehaviour
{
    [Header("References")]
    public PlayerInventory playerInventory;
    public Image[] slotIcons;
    public TextMeshProUGUI[] quantityTexts;
    public TextMeshProUGUI[] tagTexts; 
    public Image selectionHighlight;

    [Header("Animation Settings")]
    public float moveSpeed = 15f;
    public float sizeLerpSpeed = 15f;
    public Vector2 highlightPadding = new Vector2(10f, 10f);

    private RectTransform[] slotTransforms;
    private Vector2 targetPosition;
    private Vector2 targetSize;

    void OnEnable() => PlayerInventory.onInventoryChanged += UpdateUI;
    void OnDisable() => PlayerInventory.onInventoryChanged -= UpdateUI;

    void Start()
    {
        if (playerInventory == null)
            playerInventory = FindObjectOfType<PlayerInventory>();

        slotTransforms = new RectTransform[slotIcons.Length];
        for (int i = 0; i < slotIcons.Length; i++)
            slotTransforms[i] = slotIcons[i].GetComponent<RectTransform>();

        selectionHighlight.gameObject.SetActive(false);
        UpdateUI();
    }

    void Update()
    {
        // Smoothly animate selection highlight
        if (selectionHighlight.gameObject.activeSelf)
        {
            selectionHighlight.rectTransform.position = Vector3.Lerp(
                selectionHighlight.rectTransform.position,
                new Vector3(targetPosition.x, targetPosition.y, selectionHighlight.rectTransform.position.z),
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

        // Update selection highlight
        if (playerInventory.selectedSlotIndex >= 0 && playerInventory.selectedSlotIndex < slotTransforms.Length)
        {
            selectionHighlight.gameObject.SetActive(true);
            RectTransform targetSlot = slotTransforms[playerInventory.selectedSlotIndex];
            targetPosition = targetSlot.position;
            targetSize = targetSlot.sizeDelta + highlightPadding;
        }
        else
        {
            selectionHighlight.gameObject.SetActive(false);
        }

        // Update each slot
        for (int i = 0; i < slotIcons.Length; i++)
        {
            if (i >= playerInventory.items.Count || playerInventory.items[i].tagName == "")
            {
                slotIcons[i].enabled = false;
                quantityTexts[i].text = "";
                tagTexts[i].text = "";
                continue;
            }

            // Show item icon
            slotIcons[i].sprite = playerInventory.items[i].icon;
            slotIcons[i].enabled = true;

            // Show quantity
            quantityTexts[i].text = playerInventory.items[i].quantity > 1
                ? playerInventory.items[i].quantity.ToString()
                : "";

            // Show tag name
            tagTexts[i].text = playerInventory.items[i].tagName;
        }
    }
}
