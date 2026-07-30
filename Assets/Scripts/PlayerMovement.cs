using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(StaminaSystem))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Camera & Look Settings")]
    public Camera playerCamera;
    public float lookSpeed = 2f;
    public float lookXLimit = 45f;

    [Header("Movement Settings")]
    public float walkSpeed = 6f;
    public float runSpeed = 12f;
    public float crouchSpeed = 3f;
    public float gravity = 19.62f; // Keep positive here; we subtract it cleanly below

    [Header("Crouch Settings")]
    public float defaultHeight = 2f;
    public float crouchHeight = 1f;

    [Header("Noise")]
    [SerializeField] private NoiseEmitter noiseEmitter;
    [SerializeField] private float stepIntervalWalk = 0.5f;
    [SerializeField] private float stepIntervalRun = 0.35f;
    [SerializeField] private float runNoiseMultiplier = 1.8f;

    private Vector3 moveDirection = Vector3.zero;
    private float rotationX = 0;
    private CharacterController characterController;
    private StaminaSystem staminaSystem;

    private bool canMove = true;
    private float lastStepTime = -10f;

    // Store original speeds so crouching doesn't permanently overwrite them
    private float baseWalkSpeed;
    private float baseRunSpeed;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        staminaSystem = GetComponent<StaminaSystem>();

        baseWalkSpeed = walkSpeed;
        baseRunSpeed = runSpeed;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (noiseEmitter == null)
            noiseEmitter = GetComponent<NoiseEmitter>() ?? gameObject.AddComponent<NoiseEmitter>();
    }

    void Update()
    {
        // 1. Calculate direction vectors
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        // 2. Movement Input
        float moveX = canMove ? Input.GetAxis("Horizontal") : 0;
        float moveZ = canMove ? Input.GetAxis("Vertical") : 0;
        bool isMoving = (Mathf.Abs(moveX) > 0.1f || Mathf.Abs(moveZ) > 0.1f);

        // 3. Stamina & Sprinting Check
        bool wantsToRun = Input.GetKey(KeyCode.LeftShift) && isMoving;
        bool isRunning = false;

        // Get current stamina BEFORE modification for accurate UI state
        float currentStam = staminaSystem.GetCurrentStamina();

        if (wantsToRun)
        {
            isRunning = staminaSystem.TrySprint(Time.deltaTime);
            // Pass the NEW stamina value after draining
            staminaSystem.staminaBarManager.SetSprinting(true, staminaSystem.GetCurrentStamina());
        }
        else
        {
            staminaSystem.RegenerateStamina(Time.deltaTime);
            // Pass the NEW stamina value after regenerating
            // Crucial: Pass 'false' for sprinting, but the bar will stay visible 
            // because the script now checks if stamina is NOT full.
            staminaSystem.staminaBarManager.SetSprinting(false, staminaSystem.GetCurrentStamina());
        }

        // 4. Calculate Horizontal Velocity
        float currentSpeed = isRunning ? runSpeed : walkSpeed;
        Vector3 horizontalMove = (forward * moveZ + right * moveX) * currentSpeed;

        // 5. Gravity & Grounding Fix
        if (characterController.isGrounded)
        {
            // Reset downward force so gravity doesn't build up endlessly
            moveDirection.y = -2f;
        }
        else
        {
            // Apply gravity steadily when airborne
            moveDirection.y -= gravity * Time.deltaTime;
        }

        // Combine horizontal movement with vertical gravity
        moveDirection.x = horizontalMove.x;
        moveDirection.z = horizontalMove.z;

        // 6. Crouch Handling
        if (Input.GetKey(KeyCode.LeftControl) && canMove)
        {
            characterController.height = crouchHeight;
            walkSpeed = crouchSpeed;
            runSpeed = crouchSpeed;
        }
        else
        {
            characterController.height = defaultHeight;
            walkSpeed = baseWalkSpeed;
            runSpeed = baseRunSpeed;
        }

        // 7. Execute Movement
        characterController.Move(moveDirection * Time.deltaTime);

        // 8. Footstep & Noise Emission
        if (noiseEmitter != null && characterController.isGrounded && isMoving)
        {
            float interval = isRunning ? stepIntervalRun : stepIntervalWalk;
            if (Time.time - lastStepTime >= interval)
            {
                float multiplier = isRunning ? runNoiseMultiplier : 1f;
                noiseEmitter.EmitNoise(multiplier, true);
                lastStepTime = Time.time;
            }
        }

        // 9. Camera & Player Rotation
        if (canMove)
        {
            rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
        }
    }
}