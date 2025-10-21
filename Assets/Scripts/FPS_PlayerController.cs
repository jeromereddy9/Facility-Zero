using UnityEngine;

[RequireComponent(typeof(CharacterController))]
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

    [Header("Arms/Weapon Settings")]
    public Transform pistolArms;
    public Transform shotgunArms;
    public Vector3 armsOffset = Vector3.zero;

    [Header("Head Bob & FOV")]
    public float bobFrequency = 6f;
    public float bobAmplitude = 0.02f;
    public Camera cam;
    public float runFOV = 75f;
    public float fovLerpSpeed = 5f;

    [Header("Input")]
    public FacilityZero.Manager.FPInputManager inputManager;

    // --- private ---
    private float xRotation = 0f;
    private Vector2 currentMouseDelta;
    private Vector2 currentMouseDeltaVelocity;
    private Vector3 velocity;
    private float bobTimer;
    private float defaultFOV;

    private Transform activeArms => shotgunArms.gameObject.activeSelf ? shotgunArms : pistolArms;

    private void Start()
    {
        // Auto-get references
        if (cameraTransform == null)
            cameraTransform = GetComponentInChildren<Camera>().transform;
        if (cam == null)
            cam = cameraTransform.GetComponent<Camera>();
        if (characterController == null)
            characterController = GetComponent<CharacterController>();
        if (inputManager == null)
            inputManager = GetComponent<FacilityZero.Manager.FPInputManager>();

        if (characterController == null)
        {
            characterController = gameObject.AddComponent<CharacterController>();
            characterController.height = 1.8f;
            characterController.radius = 0.3f;
            characterController.center = new Vector3(0, 0.9f, 0);
        }

        // Lock cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        defaultFOV = cam.fieldOfView;
    }

    private void Update()
    {
        if (inputManager == null) return;

        HandleMouseLook();
        HandleMovement();
        HandleHeadBob();
        HandleFOV();
        UpdateArms();
    }

    private void HandleMouseLook()
    {
        Vector2 lookInput = inputManager.Look;

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
        Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;

        float currentSpeed = inputManager.Run ? runSpeed : walkSpeed;

        characterController.Move(move * currentSpeed * Time.deltaTime);

        // Apply gravity
        if (characterController.isGrounded && velocity.y < 0)
            velocity.y = -2f;

        velocity.y += gravity * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);
    }

    private void HandleHeadBob()
    {
        if (activeArms == null) return;

        if (inputManager.Move != Vector2.zero)
        {
            float speedFactor = inputManager.Run ? 1.0f : 0.5f;
            bobTimer += Time.deltaTime * bobFrequency * speedFactor;

            float bobOffsetY = Mathf.Sin(bobTimer) * bobAmplitude * speedFactor;
            float bobOffsetX = Mathf.Cos(bobTimer * 0.5f) * (bobAmplitude * 0.5f);

            activeArms.localPosition = armsOffset + new Vector3(bobOffsetX, bobOffsetY, 0);
        }
        else
        {
            bobTimer = 0f;
            activeArms.localPosition = armsOffset;
        }
    }

    private void HandleFOV()
    {
        if (cam == null) return;
        float targetFOV = inputManager.Run ? runFOV : defaultFOV;
        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetFOV, fovLerpSpeed * Time.deltaTime);
    }

    private void UpdateArms()
    {
        if (activeArms == null || cameraTransform == null) return;

        // Make active arms follow the camera rotation
        activeArms.rotation = cameraTransform.rotation;
    }

    private void OnDisable()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
