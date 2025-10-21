using UnityEngine;

public class FPSController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 5f;
    public float runSpeed = 8f;
    public float gravity = -9.81f;

    [Header("Look Settings")]
    [Range(-90f, 90f)]
    public float maxLookUpAngle = 40f;
    [Range(0f, 110f)]
    public float maxLookDownAngle = 55f;
    public float mouseSensitivity = 2f;
    public float mouseSmoothTime = 0.05f; 

    [Header("References")]
    public Transform cameraTransform;
    public CharacterController characterController;

    [Header("Input Reference")]
    public FacilityZero.Manager.FPInputManager inputManager;

    private float xRotation = 0f;
    private Vector2 currentMouseDelta;
    private Vector2 currentMouseDeltaVelocity;

    private Vector3 velocity;

    void Start()
    {
        // Auto-get references if not set
        if (cameraTransform == null)
            cameraTransform = GetComponentInChildren<Camera>().transform;

        if (characterController == null)
            characterController = GetComponent<CharacterController>();

        if (inputManager == null)
            inputManager = GetComponent<FacilityZero.Manager.FPInputManager>();

        // Ensure we have a CharacterController
        if (characterController == null)
        {
            characterController = gameObject.AddComponent<CharacterController>();
            characterController.height = 1.8f;
            characterController.radius = 0.3f;
            characterController.center = new Vector3(0, 0.9f, 0);
        }

        // Lock and hide cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Reset camera rotation
        if (cameraTransform != null)
            cameraTransform.localRotation = Quaternion.identity;
    }

    void Update()
    {
        if (inputManager == null)
        {
            Debug.LogWarning("InputManager not found!");
            return;
        }

        HandleMouseLook();
        HandleMovement();
    }

    private void HandleMouseLook()
    {
        Vector2 lookInput = inputManager.Look;

        // Smooth the mouse input for a better feel
        Vector2 targetMouseDelta = lookInput * mouseSensitivity * 0.1f;
        currentMouseDelta = Vector2.SmoothDamp(currentMouseDelta, targetMouseDelta,
                                               ref currentMouseDeltaVelocity, mouseSmoothTime);

        float mouseX = currentMouseDelta.x;
        float mouseY = currentMouseDelta.y;

        // Rotate player body (Y axis)
        transform.Rotate(Vector3.up * mouseX);

        // Rotate camera (X axis)
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -maxLookUpAngle, maxLookDownAngle);

        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }

    private void HandleMovement()
    {
        Vector2 moveInput = inputManager.Move;
        float x = moveInput.x;
        float z = moveInput.y;

        // Calculate movement direction relative to player
        Vector3 move = (transform.right * x) + (transform.forward * z);

        // Apply run speed if running
        float currentSpeed = inputManager.Run ? runSpeed : walkSpeed;

        // Move horizontally
        characterController.Move(move * currentSpeed * Time.deltaTime);

        // Apply gravity
        if (characterController.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // keep grounded
        }
        velocity.y += gravity * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);
    }

    private void OnDisable()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
