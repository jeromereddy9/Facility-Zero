using System;
using UnityEngine;
using UnityEngine.Animations.Rigging;

[System.Serializable]
public class ArmRig
{
    [Header("IK Constraints")]
    public TwoBoneIKConstraint armIK;
    public MultiRotationConstraint shoulderConstraint;

    [Header("Targets")]
    public Transform handTarget;
    public Transform elbowTarget;

    [Header("Settings")]
    [Range(0f, 1f)] public float ikWeight = 1f;
    [Range(0f, 1f)] public float elbowWeight = 1f;
    [Range(0f, 1f)] public float shoulderWeight = 0.3f;

    [Header("Offsets")]
    public Vector3 handPositionOffset;
    public Vector3 handRotationOffset;
    public Vector3 elbowPositionOffset;
}

public class WeaponIKRigging : MonoBehaviour
{
    [Header("Arm Rigs")]
    public ArmRig leftArm;
    public ArmRig rightArm;

    [Header("Gun Setup")]
    public Transform gunTransform;
    public Transform leftGripPoint;
    public Transform rightGripPoint;

    [Header("Settings")]
    public float transitionSpeed = 5f;
    [Range(0f, 1f)] public float globalWeight = 1f;
    public bool enableIK = true;

    [Header("Debug")]
    public bool showDebugGizmos = false;

    private Animator animator;
    private RigBuilder rigBuilder;
    private bool isInCombat = false;
    private float currentWeight = 0f;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        rigBuilder = GetComponent<RigBuilder>();

        if (rigBuilder == null)
            rigBuilder = gameObject.AddComponent<RigBuilder>();
    }

    private void Start()
    {
        SetupRigConstraints();
        rigBuilder.Build();
    }

    private void Update()
    {
        if (!enableIK) return;

        // Check combat state
        bool combatState = animator.GetBool("IsInCombat");
        if (combatState != isInCombat)
            isInCombat = combatState;

        // Smooth weight transition
        float targetWeight = isInCombat ? globalWeight : 0f;
        currentWeight = Mathf.Lerp(currentWeight, targetWeight, Time.deltaTime * transitionSpeed);

        // Update IK weights
        UpdateIKWeights();

        // Update targets if in combat
        if (isInCombat && gunTransform != null)
            UpdateGunTargets();
    }

    private void SetupRigConstraints()
    {
        SetupArmConstraint(leftArm, "Left");
        SetupArmConstraint(rightArm, "Right");
    }

    private void SetupArmConstraint(ArmRig arm, string armName)
    {
        if (arm.armIK == null)
        {
            Debug.LogWarning($"{armName} arm TwoBoneIKConstraint not assigned!");
            return;
        }

        var data = arm.armIK.data;
        data.target = arm.handTarget;
        data.hint = arm.elbowTarget;
        arm.armIK.data = data;

        arm.armIK.weight = 0f;
    }

    private void UpdateIKWeights()
    {
        if (leftArm.armIK != null)
            leftArm.armIK.weight = currentWeight * leftArm.ikWeight;
        if (rightArm.armIK != null)
            rightArm.armIK.weight = currentWeight * rightArm.ikWeight;

        if (leftArm.shoulderConstraint != null)
            leftArm.shoulderConstraint.weight = currentWeight * leftArm.shoulderWeight;
        if (rightArm.shoulderConstraint != null)
            rightArm.shoulderConstraint.weight = currentWeight * rightArm.shoulderWeight;
    }

    private void UpdateGunTargets()
    {
        UpdateHandTarget(leftArm, leftGripPoint);
        UpdateHandTarget(rightArm, rightGripPoint);

        // Update elbow positions automatically
        UpdateElbowTargets();
    }

    private void UpdateHandTarget(ArmRig arm, Transform gripPoint)
    {
        if (arm.handTarget == null || gripPoint == null) return;

        Vector3 targetPos = gripPoint.position + gripPoint.TransformDirection(arm.handPositionOffset);

        // Automatic rotation: forward along gun, up along gun
        Quaternion targetRot = Quaternion.LookRotation(gripPoint.forward, gripPoint.up)
                               * Quaternion.Euler(arm.handRotationOffset);

        arm.handTarget.position = targetPos;
        arm.handTarget.rotation = targetRot;
    }

    private void UpdateElbowTargets()
    {
        UpdateElbowTarget(leftArm, true);
        UpdateElbowTarget(rightArm, false);
    }

    private void UpdateElbowTarget(ArmRig arm, bool isLeft)
    {
        if (arm.elbowTarget == null || arm.handTarget == null) return;

        Transform shoulder = isLeft ? animator.GetBoneTransform(HumanBodyBones.LeftUpperArm)
                                    : animator.GetBoneTransform(HumanBodyBones.RightUpperArm);
        if (shoulder == null) return;

        Vector3 shoulderPos = shoulder.position;
        Vector3 handPos = arm.handTarget.position;

        // Midpoint
        Vector3 mid = (shoulderPos + handPos) * 0.5f;

        // Plane perpendicular to shoulder-hand vector
        Vector3 armDir = (handPos - shoulderPos).normalized;
        Vector3 up = Vector3.up;
        Vector3 lateral = Vector3.Cross(armDir, up).normalized;

        float side = isLeft ? -1f : 1f;
        Vector3 elbowPos = mid + lateral * 0.2f * side + Vector3.down * 0.05f;

        arm.elbowTarget.position = elbowPos + arm.elbowPositionOffset;
    }

    public void SetIKEnabled(bool enabled) => enableIK = enabled;
    public void SetGlobalWeight(float weight) => globalWeight = Mathf.Clamp01(weight);
    public void SetArmWeight(bool isLeft, float weight)
    {
        if (isLeft) leftArm.ikWeight = Mathf.Clamp01(weight);
        else rightArm.ikWeight = Mathf.Clamp01(weight);
    }

    private void OnDrawGizmos()
    {
        if (!showDebugGizmos) return;

        DrawArmDebug(leftArm, Color.red);
        DrawArmDebug(rightArm, Color.blue);
    }

    private void DrawArmDebug(ArmRig arm, Color color)
    {
        Gizmos.color = color;
        if (arm.handTarget != null)
        {
            Gizmos.DrawWireSphere(arm.handTarget.position, 0.03f);
            Gizmos.DrawRay(arm.handTarget.position, arm.handTarget.forward * 0.1f);
        }
        if (arm.elbowTarget != null)
            Gizmos.DrawWireCube(arm.elbowTarget.position, Vector3.one * 0.02f);
        if (arm.handTarget != null && arm.elbowTarget != null)
        {
            Gizmos.color = color * 0.5f;
            Gizmos.DrawLine(arm.elbowTarget.position, arm.handTarget.position);
        }
    }

    private void OnValidate()
    {
        globalWeight = Mathf.Clamp01(globalWeight);
        leftArm.ikWeight = Mathf.Clamp01(leftArm.ikWeight);
        rightArm.ikWeight = Mathf.Clamp01(rightArm.ikWeight);
        leftArm.elbowWeight = Mathf.Clamp01(leftArm.elbowWeight);
        rightArm.elbowWeight = Mathf.Clamp01(rightArm.elbowWeight);
        leftArm.shoulderWeight = Mathf.Clamp01(leftArm.shoulderWeight);
        rightArm.shoulderWeight = Mathf.Clamp01(rightArm.shoulderWeight);
        transitionSpeed = Mathf.Max(0.1f, transitionSpeed);
    }
}
