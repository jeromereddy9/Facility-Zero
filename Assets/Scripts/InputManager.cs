using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.UIElements.UxmlAttributeDescription;

namespace FacilityZero.Manager
{
    [RequireComponent(typeof(PlayerInput))]
    public class InputManager : MonoBehaviour
    {
        // Public properties for access by other scripts
        public Vector2 Move { get; private set; }
        public Vector2 Look { get; private set; }
        public bool Run { get; private set; }
        public bool CombatTogglePressed { get; set; }
        public bool Shoot { get; private set; }
        public bool Interact { get; private set; }
        public bool Reload { get; private set; }
        public bool UseItem { get; private set; }

        // Private Input System references
        private PlayerInput playerInput;
        private InputAction moveAction;
        private InputAction lookAction;
        private InputAction runAction;
        private InputAction combatModeAction;
        private InputAction shootAction;
        private InputAction interactAction;
        private InputAction reloadAction;
        private InputAction UseItemAction;

        private void Awake()
        {
            playerInput = GetComponent<PlayerInput>();

            // Get current action map
            var actionMap = playerInput.currentActionMap;

            moveAction = actionMap.FindAction("Move");
            lookAction = actionMap.FindAction("Look");
            runAction = actionMap.FindAction("Run");
            combatModeAction = actionMap.FindAction("Combat Mode");
            shootAction = actionMap.FindAction("Shoot");
            interactAction = actionMap.FindAction("Interact");
            reloadAction = actionMap.FindAction("Reload");
            UseItemAction = actionMap.FindAction("Use Item");

            HideCursor();
        }

        private void OnEnable()
        {
            // Subscribe to events
            moveAction.performed += OnMove;
            moveAction.canceled += OnMove;

            lookAction.performed += OnLook;
            lookAction.canceled += OnLook;

            runAction.performed += OnRun;
            runAction.canceled += OnRun;

            shootAction.performed += OnShoot;
            shootAction.canceled += OnShoot;

            interactAction.performed += OnInteract;
            interactAction.canceled += OnInteract;

            reloadAction.performed += OnReload;
            reloadAction.canceled += OnReload;

            UseItemAction.performed += OnUseItem;
            UseItemAction.canceled += OnUseItem;

            combatModeAction.performed += OnCombatMode;

            playerInput.actions.Enable();
        }

        private void OnDisable()
        {
            // Unsubscribe from events
            moveAction.performed -= OnMove;
            moveAction.canceled -= OnMove;

            lookAction.performed -= OnLook;
            lookAction.canceled -= OnLook;

            runAction.performed -= OnRun;
            runAction.canceled -= OnRun;

            shootAction.performed -= OnShoot;
            shootAction.canceled -= OnShoot;

            interactAction.performed -= OnInteract;
            interactAction.canceled -= OnInteract;

            reloadAction.performed -= OnReload;
            reloadAction.canceled -= OnReload;

            UseItemAction.performed -= OnUseItem;
            UseItemAction.canceled -= OnUseItem;

            combatModeAction.performed -= OnCombatMode;

            playerInput.actions.Disable();
        }

        // Input callbacks
        private void OnMove(InputAction.CallbackContext context)
        {
            Move = context.ReadValue<Vector2>();
        }

        private void OnLook(InputAction.CallbackContext context)
        {
            Look = context.ReadValue<Vector2>();
        }

        private void OnRun(InputAction.CallbackContext context)
        {
            Run = context.ReadValueAsButton();
        }

        private void OnShoot(InputAction.CallbackContext context)
        {
            Shoot = context.ReadValueAsButton();
        }

        private void OnInteract(InputAction.CallbackContext context)
        {
            Interact = context.ReadValueAsButton();
        }

        private void OnCombatMode(InputAction.CallbackContext context)
        {
            CombatTogglePressed = true;
        }

        private void OnReload(InputAction.CallbackContext context)
        {
            Reload = context.ReadValueAsButton();
        }

        private void OnUseItem(InputAction.CallbackContext context)
        {
            UseItem = context.ReadValueAsButton();
        }

        private void HideCursor()
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
}
