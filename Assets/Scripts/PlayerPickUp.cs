using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerPickup : MonoBehaviour
{
    public float pickupRange = 5f;
    public LayerMask pickupLayerMask = -1; // Default to all layers

    void Update()
    {
        Keyboard keyboard = Keyboard.current;
        if (keyboard == null) return;

        if (keyboard.fKey.wasPressedThisFrame)
        {
            TryPickUp();
        }
    }

    void TryPickUp()
    {
        Debug.Log("=== PICKUP ATTEMPT ===");

        Camera cam = Camera.main;
        if (cam == null)
        {
            Debug.LogError("No Main Camera found in scene!");
            return;
        }

        Ray ray = new Ray(cam.transform.position, cam.transform.forward);
        RaycastHit hit;

        // Visual debug ray
        Debug.DrawRay(ray.origin, ray.direction * pickupRange, Color.green, 2f);

        if (Physics.Raycast(ray, out hit, pickupRange, pickupLayerMask))
        {
            Debug.Log("Raycast HIT: " + hit.collider.name);

            ObjectPickUp pickup = hit.collider.GetComponent<ObjectPickUp>();
            if (pickup != null)
            {
                Debug.Log("Found ObjectPickUp: " + pickup.itemName);

                // Try different ways to find the inventory
                PlayerInventory inventory = GetComponent<PlayerInventory>();
                if (inventory == null)
                    inventory = GetComponentInParent<PlayerInventory>();
                if (inventory == null)
                    inventory = FindFirstObjectByType<PlayerInventory>();

                if (inventory != null)
                {
                    Debug.Log("PlayerInventory found! Adding item...");
                    pickup.PickUp(inventory);
                }
                else
                {
                    Debug.LogError("PlayerInventory NOT found anywhere!");
                }
            }
            else
            {
                Debug.Log("No ObjectPickUp component on: " + hit.collider.name);
            }
        }
        else
        {
            Debug.Log("Raycast missed - nothing in range or wrong layers");
        }
    }

    // Alternative: Simple trigger-based pickup
    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger entered with: " + other.name);

        ObjectPickUp pickup = other.GetComponent<ObjectPickUp>();
        if (pickup != null)
        {
            Debug.Log("Trigger found ObjectPickUp: " + pickup.itemName);

            PlayerInventory inventory = GetComponent<PlayerInventory>();
            if (inventory != null)
            {
                pickup.PickUp(inventory);
            }
        }
    }
}