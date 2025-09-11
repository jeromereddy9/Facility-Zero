using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

namespace FacilityZero.Manager
{
    public class InputManager : MonoBehaviour
    {
        [SerializeField] private PlayerInput PlayerInput;

        public Vector2 Move { get; private set; }

        public Vector2 Look { get; private set; }

        public bool Run { get; private set; }

        private InputActionMap actionMap;

        private InputAction moveAction;
        private InputAction lookAction;
        private InputAction runAction;

        private void Awake()
        {
            actionMap = PlayerInput.currentActionMap;

            moveAction = actionMap.FindAction("Move");
            lookAction = actionMap.FindAction("Look");
            runAction = actionMap.FindAction("Run");

            moveAction.performed += onMove;
            lookAction.performed += onLook;
            runAction.performed += onRun;

            moveAction.canceled += onMove;
            lookAction.canceled += onLook;
            runAction.canceled += onRun;

        }

        private void onMove(InputAction.CallbackContext context)
        {
            Move = context.ReadValue<Vector2>();
        }

        private void onLook(InputAction.CallbackContext context)
        {
            Look = context.ReadValue<Vector2>();
        }

        private void onRun(InputAction.CallbackContext context)
        {
            Run = context.ReadValueAsButton();
        }

        private void OnEnable()
        {
            actionMap.Enable();
        }

        private void OnDisable()
        {
            actionMap.Disable();    
        }
    }

}