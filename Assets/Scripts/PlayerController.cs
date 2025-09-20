using UnityEngine;
using FacilityZero.Manager;

namespace FacilityZero.PlayerControl
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(InputManager))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Camera Settings")]
        [SerializeField] private Transform CameraRoot;
        [SerializeField] private Transform Camera;
        [SerializeField] private float UpperLimit = -40.0f;
        [SerializeField] private float BottomLimit = 55.0f;
        [SerializeField] private float MouseSensitivity = 2.0f;
        [SerializeField] private float runFOV = 75f;
        [SerializeField] private float fovLerpSpeed = 5f;
        [SerializeField] private float bobFrequency = 6f;
        [SerializeField] private float bobAmplitude = 0.05f;
        [SerializeField] private float nearClip = 0.05f;

        [Header("Movement Settings")]
        [SerializeField] private float AnimationBlendSpeed = 8.9f;
        [SerializeField] private float WalkSpeed = 2.0f;
        [SerializeField] private float RunSpeed = 6.0f;

        [Header("Combat Settings")]
        [SerializeField] private GameObject gun; 

        private Camera cam;
        private float defaultFOV;
        private float bobTimer;

        private Rigidbody playerRB;
        private InputManager inputManager;
        //private WeaponIK weaponIK;
        private Animator animator;
        private bool hasAnimator;

        private int x_velocityHash;
        private int y_velocityHash;

        private float x_Rotation;
        private Vector2 currentVel;

        private void Start()
        {
            hasAnimator = TryGetComponent(out animator);
            playerRB = GetComponent<Rigidbody>();
            inputManager = GetComponent<InputManager>();
            //weaponIK = GetComponent<WeaponIK>();

            cam = Camera.GetComponent<Camera>();
            cam.nearClipPlane = nearClip;
            defaultFOV = cam.fieldOfView;
            gun.SetActive(false);

            x_velocityHash = Animator.StringToHash("X_Velocity");
            y_velocityHash = Animator.StringToHash("Y_Velocity");
        }

        private void FixedUpdate() => Move();

        private void LateUpdate()
        {
            CameraMovement();
            HandleFOV();
            HandleHeadBob();
            ToggleCombatMode();
        }

        private void ToggleCombatMode()
        {
            if (inputManager.CombatTogglePressed)
            {
                bool current = animator.GetBool("IsInCombat");
                animator.SetBool("IsInCombat", !current);

                if (gun != null)
                {
                    gun.SetActive(!current);
                }

                inputManager.CombatTogglePressed = false;
            }
        }


        private void HandleFOV()
        {
            float targetFOV = inputManager.Run ? runFOV : defaultFOV;
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetFOV, fovLerpSpeed * Time.deltaTime);
        }

        private void HandleHeadBob()
        {
            Vector3 basePos = CameraRoot.position;

            if (inputManager.Move != Vector2.zero)
            {
                float speedFactor = inputManager.Run ? 1.0f : 0.5f;
                bobTimer += Time.deltaTime * bobFrequency * speedFactor;

                float bobOffsetY = Mathf.Sin(bobTimer) * bobAmplitude * speedFactor;
                float bobOffsetX = Mathf.Cos(bobTimer * 0.5f) * (bobAmplitude * 0.5f);

                Camera.position = basePos + new Vector3(bobOffsetX, bobOffsetY, 0);
            }
            else
            {
                bobTimer = 0f;
                Camera.position = basePos;
            }
        }

        private void Move()
        {
            float targetSpeed = inputManager.Run ? RunSpeed : WalkSpeed;
            if (inputManager.Move == Vector2.zero)
                targetSpeed = 0.0f;

            currentVel.x = Mathf.Lerp(currentVel.x, inputManager.Move.x * targetSpeed, AnimationBlendSpeed * Time.deltaTime);
            currentVel.y = Mathf.Lerp(currentVel.y, inputManager.Move.y * targetSpeed, AnimationBlendSpeed * Time.deltaTime);

            Vector3 targetVelocity = transform.TransformDirection(new Vector3(currentVel.x, 0, currentVel.y));
            Vector3 velocityChange = targetVelocity - playerRB.velocity;
            velocityChange.y = 0;

            playerRB.AddForce(velocityChange, ForceMode.VelocityChange);

            if (hasAnimator)
            {
                animator.SetFloat(x_velocityHash, currentVel.x);
                animator.SetFloat(y_velocityHash, currentVel.y);
            }
        }

        private void CameraMovement()
        {
            float mouseX = inputManager.Look.x;
            float mouseY = inputManager.Look.y;

            x_Rotation -= mouseY * MouseSensitivity * Time.deltaTime;
            x_Rotation = Mathf.Clamp(x_Rotation, UpperLimit, BottomLimit);

            Camera.localRotation = Quaternion.Euler(x_Rotation, 0, 0);
            transform.Rotate(Vector3.up, mouseX * MouseSensitivity * Time.deltaTime);
        }
    }
}