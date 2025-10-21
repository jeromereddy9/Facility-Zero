using UnityEngine;
using TMPro;
using FacilityZero.Manager;
using FacilityZero.PlayerInventory;

namespace FacilityZero.IntercomController
{
    public class Intercom : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform door;
        [SerializeField] private float doorMoveAmount = 3f;
        [SerializeField] private float doorMoveSpeed = 2f;
        [SerializeField] private TMP_Text popupText;
        [SerializeField] private Camera playerCamera;

        [Header("Keycard Settings")]
        [SerializeField] private string requiredKeyTag = "Access Keycard lvl1";
        [SerializeField] private int requiredKeyCount = 1; // number of keycards required

        private bool isPlayerLooking = false;
        private bool isDoorOpen = false;
        private Vector3 doorClosedPos;
        private Vector3 doorOpenPos;
        private FPInputManager inputManager;

        void Start()
        {
            inputManager = GetComponentInParent<FPInputManager>() ?? FindObjectOfType<FPInputManager>();
            if (inputManager == null)
                Debug.LogError("Intercom: No FPInputManager found!");

            if (door != null)
            {
                doorClosedPos = door.position;
                doorOpenPos = doorClosedPos + Vector3.up * doorMoveAmount;
            }

            if (popupText != null)
                popupText.gameObject.SetActive(false);
        }

        void Update()
        {
            CheckPlayerLook();

            if (isPlayerLooking && inputManager != null && inputManager.InteractPressedThisFrame)
            {
                TryOpenDoor();
            }

            if (isDoorOpen && door != null)
            {
                door.position = Vector3.MoveTowards(door.position, doorOpenPos, doorMoveSpeed * Time.deltaTime);
            }
        }

        void CheckPlayerLook()
        {
            Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
            PlayerInventory.PlayerInventory inventory = FindObjectOfType<PlayerInventory.PlayerInventory>();
            int keyCount = inventory != null ? inventory.CountItemsWithTag(requiredKeyTag) : 0;

            if (Physics.Raycast(ray, out RaycastHit hit, 3f))
            {
                if (hit.transform.CompareTag("Intercom"))
                {
                    if (!isPlayerLooking)
                    {
                        isPlayerLooking = true;
                        popupText.gameObject.SetActive(true);
                    }

                    // Update popup with keycard count
                    if (keyCount >= requiredKeyCount)
                        popupText.text = $"Press I to use Intercom ({keyCount}/{requiredKeyCount} keycards)";
                    else
                        popupText.text = $"Need {requiredKeyCount} keycards ({keyCount}/{requiredKeyCount})";

                    return;
                }
            }

            if (isPlayerLooking)
            {
                isPlayerLooking = false;
                popupText.gameObject.SetActive(false);
            }
        }

        void TryOpenDoor()
        {
            PlayerInventory.PlayerInventory inventory = FindObjectOfType<PlayerInventory.PlayerInventory>();
            if (inventory == null) return;

            int keyCount = inventory.CountItemsWithTag(requiredKeyTag);

            if (keyCount >= requiredKeyCount)
            {
                isDoorOpen = true;
                popupText.gameObject.SetActive(false);
                Debug.Log($"Door opened! Player has {keyCount}/{requiredKeyCount} keycards.");
            }
            else
            {
                popupText.text = $"Access Denied: {requiredKeyCount} keycards required ({keyCount}/{requiredKeyCount})";
                Debug.Log($"Not enough keycards! Player has {keyCount}/{requiredKeyCount}.");
            }
        }
    }
}
