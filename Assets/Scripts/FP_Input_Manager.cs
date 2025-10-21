using UnityEngine;
using UnityEngine.InputSystem;

namespace FacilityZero.Manager
{
    [RequireComponent(typeof(PlayerInput))]
    public class FPInputManager : MonoBehaviour
    {
        public Vector2 Move { get; private set; }
        public Vector2 Look { get; private set; }
        public bool Run { get; private set; }
        public bool CycleWeapons { get; set; }
        public bool Shoot { get; private set; }
        public bool Reload { get; private set; }
        public bool Use { get; private set; }      // For items like medkits
        public bool Interact { get; private set; } // For world interactions

        // Was pressed this frame helpers
        public bool ShootPressedThisFrame => shootAction != null && shootAction.WasPressedThisFrame();
        public bool ReloadPressedThisFrame => reloadAction != null && reloadAction.WasPressedThisFrame();
        public bool UsePressedThisFrame => useAction != null && useAction.WasPressedThisFrame();
        public bool InteractPressedThisFrame => interactAction != null && interactAction.WasPressedThisFrame();

        private PlayerInput playerInput;
        private InputAction moveAction, lookAction, runAction;
        private InputAction cycleWeaponsAction, shootAction, reloadAction, useAction, interactAction;

        private void Awake()
        {
            playerInput = GetComponent<PlayerInput>();
            var actionMap = playerInput.currentActionMap;

            moveAction = actionMap.FindAction("Move");
            lookAction = actionMap.FindAction("Look");
            runAction = actionMap.FindAction("Run");
            cycleWeaponsAction = actionMap.FindAction("CycleWeapons");
            shootAction = actionMap.FindAction("Shoot");
            reloadAction = actionMap.FindAction("Reload");
            useAction = actionMap.FindAction("Use");
            interactAction = actionMap.FindAction("Interact");

            if (cycleWeaponsAction == null)
                Debug.LogError("CycleWeapons action not found in " + actionMap.name);
            if (useAction == null)
                Debug.LogWarning("Use action not found in " + actionMap.name);
            if (interactAction == null)
                Debug.LogWarning("Interact action not found in " + actionMap.name);

            HideCursor();
        }

        private void OnEnable()
        {
            moveAction.performed += OnMove;
            moveAction.canceled += OnMove;

            lookAction.performed += OnLook;
            lookAction.canceled += OnLook;

            runAction.performed += OnRun;
            runAction.canceled += OnRun;

            shootAction.performed += OnShoot;
            shootAction.canceled += OnShoot;

            reloadAction.performed += OnReload;
            reloadAction.canceled += OnReload;

            cycleWeaponsAction.performed += OnCycleWeapons;

            if (useAction != null)
            {
                useAction.performed += OnUse;
                useAction.canceled += OnUse;
            }

            if (interactAction != null)
            {
                interactAction.performed += OnInteract;
                interactAction.canceled += OnInteract;
            }

            playerInput.actions.Enable();
        }

        private void OnDisable()
        {
            moveAction.performed -= OnMove;
            moveAction.canceled -= OnMove;

            lookAction.performed -= OnLook;
            lookAction.canceled -= OnLook;

            runAction.performed -= OnRun;
            runAction.canceled -= OnRun;

            shootAction.performed -= OnShoot;
            shootAction.canceled -= OnShoot;

            reloadAction.performed -= OnReload;
            reloadAction.canceled -= OnReload;

            cycleWeaponsAction.performed -= OnCycleWeapons;

            if (useAction != null)
            {
                useAction.performed -= OnUse;
                useAction.canceled -= OnUse;
            }

            if (interactAction != null)
            {
                interactAction.performed -= OnInteract;
                interactAction.canceled -= OnInteract;
            }

            playerInput.actions.Disable();
        }

        private void OnMove(InputAction.CallbackContext ctx) => Move = ctx.ReadValue<Vector2>();
        private void OnLook(InputAction.CallbackContext ctx) => Look = ctx.ReadValue<Vector2>();
        private void OnRun(InputAction.CallbackContext ctx) => Run = ctx.ReadValueAsButton();
        private void OnShoot(InputAction.CallbackContext ctx) => Shoot = ctx.ReadValueAsButton();
        private void OnReload(InputAction.CallbackContext ctx) => Reload = ctx.ReadValueAsButton();
        private void OnUse(InputAction.CallbackContext ctx) => Use = ctx.ReadValueAsButton();
        private void OnInteract(InputAction.CallbackContext ctx) => Interact = ctx.ReadValueAsButton();

        private void OnCycleWeapons(InputAction.CallbackContext ctx)
        {
            if (ctx.performed)
            {
                CycleWeapons = true;
                Debug.Log("CycleWeapons triggered!");
            }
        }

        private void HideCursor()
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
}
