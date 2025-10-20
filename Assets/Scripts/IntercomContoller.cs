using UnityEngine;
using UnityEngine.UI; 

namespace FacilityZero.IntercomController
{
    using UnityEngine;
    using UnityEngine.UI;
    using TMPro;
    using FacilityZero.Manager;

    public class Intercom : MonoBehaviour,ISavable
    {
        [Header("References")]
        [SerializeField] private Transform door;       
        [SerializeField] private float doorMoveAmount = 3f;
        [SerializeField] private float doorMoveSpeed = 2f;
        [SerializeField] private TMP_Text popupText;       
        [SerializeField] private Camera playerCamera;  
        [SerializeField] private string requiredKeyTag = "Access Keycard lvl1";

        private bool isPlayerLooking = false;
        private bool isDoorOpen = false;
        private Vector3 doorClosedPos;
        private Vector3 doorOpenPos;
        private InputManager inputManager;

        void Start()
        {
            inputManager = GetComponentInParent<InputManager>();
            if (inputManager == null)
            {
                inputManager = FindObjectOfType<InputManager>(); // fallback
                if (inputManager == null)
                {
                    Debug.LogError("Shooter: No InputManager found in parents or scene!");
                }
            }

            if (door != null)
            {
                doorClosedPos = door.position;
                doorOpenPos = doorClosedPos + Vector3.up * doorMoveAmount;
            }

            if (popupText != null)
                popupText.gameObject.SetActive(false); // hide popup at start
        }

        void Update()
        {
            CheckPlayerLook();

            if (isPlayerLooking && inputManager != null && inputManager.Interact)
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
            if (Physics.Raycast(ray, out RaycastHit hit, 3f)) // 3 units interaction range
            {
                Debug.Log("Ray hit: " + hit.transform.name); // <- debug
                if (hit.transform.CompareTag("Intercom"))
                {
                    if (!isPlayerLooking)
                    {
                        isPlayerLooking = true;
                        popupText.gameObject.SetActive(true);
                        popupText.text = "Press F to use Intercom";
                    }
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
            // Assume you have a PlayerInventory class with HasKey(string tag) method
            PlayerInventory.PlayerInventory inventory = FindObjectOfType<PlayerInventory.PlayerInventory>();

            if (inventory != null && inventory.HasItemWithTag(requiredKeyTag))
            {
                Debug.Log("Keycard accepted. Opening door...");
                isDoorOpen = true;
                popupText.gameObject.SetActive(false);
            }
            else
            {
                Debug.Log("Missing required keycard!");
                popupText.text = "Access Denied: Keycard Required";
            }
        }

        public void SaveData(GameSaveData saveData)
        {
            // You’ll need to uniquely identify each door if you have multiple
            if (!saveData.intercomStates.ContainsKey(gameObject.name))
                saveData.intercomStates.Add(gameObject.name, isDoorOpen);
            else
                saveData.intercomStates[gameObject.name] = isDoorOpen;
        }

        public void LoadData(GameSaveData saveData)
        {
            if (saveData.intercomStates.TryGetValue(gameObject.name, out bool wasOpen))
            {
                isDoorOpen = wasOpen;

                // Immediately update the door position if it was already open
                if (isDoorOpen && door != null)
                    door.position = doorOpenPos;
            }
        }
    }

}