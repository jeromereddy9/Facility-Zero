using UnityEngine;
using TMPro;

public class PickupPopupUI : MonoBehaviour
{
    [Header("UI References")]
    public GameObject popupPanel;
    public TextMeshProUGUI popupText;

    [Header("Settings")]
    public string pickupPrompt = "";
    public float checkInterval = 0.2f;
    public float popupRange = 3f;

    private Camera playerCamera;
    private float lastCheckTime;
    private ObjectPickUp currentTarget;
    private bool isInitialized = false;

    void Start()
    {
        InitializePopup();
    }

    void InitializePopup()
    {
        // Try to find camera if not set
        if (playerCamera == null)
            playerCamera = Camera.main;

        // Ensure we have the UI elements
        if (popupPanel == null)
        {
            Debug.LogError("Popup Panel is not assigned in the inspector!");
            return;
        }

        // Start with hidden popup
        popupPanel.SetActive(false);
        isInitialized = true;

        Debug.Log("Pickup Popup UI initialized");
    }

    void Update()
    {
        if (!isInitialized) return;

        // Only check periodically for efficiency
        if (Time.time - lastCheckTime >= checkInterval)
        {
            CheckForPickupObjects();
            lastCheckTime = Time.time;
        }

        // Update popup position if visible
        if (popupPanel.activeSelf && currentTarget != null)
        {
            UpdatePopupPosition();
        }
    }

    void CheckForPickupObjects()
    {
        if (playerCamera == null)
        {
            playerCamera = Camera.main;
            if (playerCamera == null) return;
        }

        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit hit;

        Debug.DrawRay(ray.origin, ray.direction * popupRange, Color.blue, checkInterval);

        if (Physics.Raycast(ray, out hit, popupRange))
        {
            ObjectPickUp pickup = hit.collider.GetComponent<ObjectPickUp>();
            if (pickup != null)
            {
                Debug.Log("Found pickup object with tag: " + hit.collider.tag);
                ShowPopup(pickup, hit.collider.tag);
                currentTarget = pickup;
                return;
            }
        }

        // If no object found, hide the popup
        if (popupPanel.activeSelf)
        {
            HidePopup();
        }
        currentTarget = null;
    }

    void ShowPopup(ObjectPickUp pickup, string objectTag)
    {
        if (popupPanel != null)
        {
            popupPanel.SetActive(true);

            // Set popup text with the object's tag name
            if (popupText != null)
            {
                // Directly concatenate the tag with the prompt
                popupText.text = pickupPrompt + objectTag;
                Debug.Log("Popup text: " + popupText.text);
            }

            Debug.Log("Showing pickup popup for object with tag: " + objectTag);
        }
    }

    public void HidePopup()
    {
        if (popupPanel != null)
        {
            popupPanel.SetActive(false);
            Debug.Log("Hiding pickup popup");
        }
        currentTarget = null;
    }

    void UpdatePopupPosition()
    {
        if (currentTarget == null || playerCamera == null) return;

        // Position the popup above the object in world space
        Vector3 worldPosition = currentTarget.transform.position + Vector3.up * 0.5f;
        Vector3 screenPosition = playerCamera.WorldToScreenPoint(worldPosition);

        // Only show if the object is in front of the camera
        if (screenPosition.z > 0)
        {
            popupPanel.transform.position = screenPosition;
        }
        else
        {
            HidePopup();
        }
    }
}