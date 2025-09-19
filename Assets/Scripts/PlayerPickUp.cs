using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerPickup : MonoBehaviour
{
    public float pickupRange = 5f;
    public LayerMask pickupLayerMask = -1;

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
        if (cam == null) return;

        Ray ray = new Ray(cam.transform.position, cam.transform.forward);
        RaycastHit hit;
        Debug.DrawRay(ray.origin, ray.direction * pickupRange, Color.green, 2f);

        if (Physics.Raycast(ray, out hit, pickupRange, pickupLayerMask))
        {
            ObjectPickUp pickup = hit.collider.GetComponent<ObjectPickUp>();
            if (pickup != null)
            {
                PlayerInventory inventory = GetComponent<PlayerInventory>();
                if (inventory == null)
                    inventory = GetComponentInParent<PlayerInventory>();
                if (inventory == null)
                    inventory = FindFirstObjectByType<PlayerInventory>();

                if (inventory != null)
                    pickup.PickUp(inventory);
            }
        }
    }
}
