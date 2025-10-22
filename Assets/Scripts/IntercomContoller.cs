using UnityEngine;
using TMPro;
using FacilityZero.Manager;
using FacilityZero.PlayerInventory;
using FacilityZero.UI;

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
        [SerializeField] private WinScreen winScreen;

        [Header("Keycard Settings")]
        [SerializeField] private string requiredKeyTag = "Access Keycard lvl1";
        [SerializeField] private int requiredKeyCount = 1;

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

            if (winScreen == null)
                winScreen = FindObjectOfType<WinScreen>(true);
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

                // ✅ Check for win condition once door is fully open
                if (Vector3.Distance(door.position, doorOpenPos) < 0.05f)
                {
                    TryTriggerWin();
                }
            }
        }

        void CheckPlayerLook()
        {
            Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
            if (Physics.Raycast(ray, out RaycastHit hit, 3f))
            {
                if (hit.transform.CompareTag("Intercom"))
                {
                    isPlayerLooking = true;
                    popupText.gameObject.SetActive(true);
                    popupText.text = "Press I to use Intercom";
                    return;
                }
            }

            isPlayerLooking = false;
            popupText.gameObject.SetActive(false);
        }

        void TryOpenDoor()
        {
            PlayerInventory.PlayerInventory inventory = FindObjectOfType<PlayerInventory.PlayerInventory>();
            if (inventory == null) return;

            int keyCount = inventory.CountItemsWithTag(requiredKeyTag);

            if (keyCount >= requiredKeyCount)
            {
                popupText.gameObject.SetActive(false);
                isDoorOpen = true;
                Debug.Log($"Door opened!");
            }
            else
            {
                popupText.text = $"Access Denied: Insufficient keycards";
                Debug.Log($"Not enough keycards! ");
            }
        }

        void TryTriggerWin()
        {
            if (door != null && door.CompareTag("Exit"))
            {
                PlayerInventory.PlayerInventory inventory = FindObjectOfType<PlayerInventory.PlayerInventory>();
                if (inventory == null) return;

                int keyCount = inventory.CountItemsWithTag(requiredKeyTag);

                if (keyCount >= 4)
                {
                    Debug.Log("Player reached the Exit with all keycards! YOU WIN!");

                    if (winScreen != null)
                    {
                        winScreen.gameObject.SetActive(true);
                        winScreen.TriggerWinScreen();

                        // ✅ Optional: freeze player movement
                        if (inputManager != null)
                            inputManager.enabled = false;
                    }
                    else
                    {
                        Debug.LogError("WinScreen reference missing in Intercom!");
                    }

                    // Ensure this only triggers once
                    isDoorOpen = false;
                }
            }
        }
    }
}
