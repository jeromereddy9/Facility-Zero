using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HotbarUI : MonoBehaviour
{
    public PlayerInventory playerInventory;
    public Image[] slotIcons;
    public TextMeshProUGUI[] quantityTexts;
    public Image selectionHighlight;

    public float moveSpeed = 15f;
    public float sizeLerpSpeed = 15f;

    private RectTransform[] slotTransforms;
    private Vector2 targetPosition;
    private Vector2 targetSize;

    void OnEnable() => PlayerInventory.onInventoryChanged += UpdateUI;
    void OnDisable() => PlayerInventory.onInventoryChanged -= UpdateUI;

    void Start()
    {
        if (playerInventory == null)
            playerInventory = FindFirstObjectByType<PlayerInventory>();

        slotTransforms = new RectTransform[slotIcons.Length];
        for (int i = 0; i < slotIcons.Length; i++)
            slotTransforms[i] = slotIcons[i].GetComponent<RectTransform>();

        selectionHighlight.gameObject.SetActive(false);
        UpdateUI();
    }

    void Update()
    {
        if (!selectionHighlight.gameObject.activeSelf) return;

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

    void UpdateUI()
    {
        if (playerInventory == null) return;

        // Highlight selected slot
        if (playerInventory.selectedSlotIndex >= 0 &&
            playerInventory.selectedSlotIndex < slotTransforms.Length &&
            slotTransforms[playerInventory.selectedSlotIndex] != null)
        {
            selectionHighlight.gameObject.SetActive(true);
            RectTransform targetSlot = slotTransforms[playerInventory.selectedSlotIndex];
            targetPosition = targetSlot.position;
            targetSize = targetSlot.sizeDelta + new Vector2(20f, 20f);
        }
        else
        {
            selectionHighlight.gameObject.SetActive(false);
        }

        // Update icons
        for (int i = 0; i < slotIcons.Length; i++)
        {
            if (i >= playerInventory.items.Count)
            {
                slotIcons[i].enabled = false;
                quantityTexts[i].text = "";
                continue;
            }

            var item = playerInventory.items[i];

            if (item.itemType != "Empty" && item.quantity > 0)
            {
                slotIcons[i].sprite = item.icon;
                slotIcons[i].enabled = true;
                quantityTexts[i].text = item.quantity > 1 ? "x" + item.quantity : "";
            }
            else
            {
                slotIcons[i].enabled = false;
                quantityTexts[i].text = "";
            }
        }
    }
}
