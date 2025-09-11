using UnityEngine;
using UnityEngine.InputSystem;

namespace FacilityZero.Manager
{
    [RequireComponent(typeof(PlayerInput))]
    public class InputManager : MonoBehaviour
    {
        public Vector2 Move { get; private set; }
        public Vector2 Look { get; private set; }
        public bool Run { get; private set; }
        public bool CombatTogglePressed { get; set; }

        private PlayerInput playerInput;
        private InputAction moveAction;
        private InputAction lookAction;
        private InputAction runAction;
        private InputAction combatModeAction;

        private void Awake()
        {
            HideCursor();

            playerInput = GetComponent<PlayerInput>();
            var actionMap = playerInput.currentActionMap;

            moveAction = actionMap.FindAction("Move");
            lookAction = actionMap.FindAction("Look");
            runAction = actionMap.FindAction("Run");
            combatModeAction = actionMap.FindAction("Combat Mode");

            // Combat toggle triggers once per press
            combatModeAction.performed += ctx => CombatTogglePressed = true;
        }

        private void OnEnable()
        {
            moveAction.performed += OnMove;
            moveAction.canceled += OnMove;

            lookAction.performed += OnLook;
            lookAction.canceled += OnLook;

            runAction.performed += OnRun;
            runAction.canceled += OnRun;

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

            playerInput.actions.Disable();
        }

        private void OnMove(InputAction.CallbackContext context) =>
            Move = context.ReadValue<Vector2>();

        private void OnLook(InputAction.CallbackContext context) =>
            Look = context.ReadValue<Vector2>();

        private void OnRun(InputAction.CallbackContext context) =>
            Run = context.ReadValueAsButton();

        private void HideCursor()
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
}
