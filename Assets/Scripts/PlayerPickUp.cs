using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerPickup : MonoBehaviour
{
    public float pickupRange = 5f;
    public LayerMask pickupLayerMask = -1;

    // Reference to the popup UI
    private PickupPopupUI pickupPopup;

    void Start()
    {
        // Find the popup UI in the scene
        pickupPopup = FindObjectOfType<PickupPopupUI>();
        if (pickupPopup == null)
        {
            Debug.LogWarning("No PickupPopupUI found in scene");
        }
    }

    void Update()
    {
        Keyboard keyboard = Keyboard.current;
        if (keyboard == null) return;

        if (keyboard.fKey.wasPressedThisFrame)
            TryPickUp();
    }

    void TryPickUp()
    {
        Camera cam = Camera.main;
        if (cam == null)
        {
            Debug.LogWarning("No main camera found");
            return;
        }

        Ray ray = new Ray(cam.transform.position, cam.transform.forward);
        RaycastHit hit;
        Debug.DrawRay(ray.origin, ray.direction * pickupRange, Color.green, 2f);

        if (Physics.Raycast(ray, out hit, pickupRange, pickupLayerMask))
        {
            Debug.Log("Raycast hit: " + hit.collider.gameObject.name);

            ObjectPickUp pickup = hit.collider.GetComponent<ObjectPickUp>();
            if (pickup != null)
            {
                Debug.Log("Found ObjectPickUp component on: " + hit.collider.gameObject.name);

                PlayerInventory inventory = GetComponent<PlayerInventory>();
                if (inventory == null)
                    inventory = GetComponentInParent<PlayerInventory>();
                if (inventory == null)
                    inventory = FindFirstObjectByType<PlayerInventory>();

                if (inventory != null)
                {
                    pickup.PickUp(inventory);

                    // Hide the popup after picking up
                    if (pickupPopup != null)
                        pickupPopup.HidePopup();
                }
                else
                {
                    Debug.LogWarning("No PlayerInventory found");
                }
            }
            else
            {
                Debug.Log("No ObjectPickUp component found on: " + hit.collider.gameObject.name);
            }
        }
        else
        {
            Debug.Log("No object hit with raycast");
        }
    }
}