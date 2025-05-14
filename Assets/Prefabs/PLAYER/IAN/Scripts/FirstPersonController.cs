using System;
using UnityEngine;

public partial class FirstPersonController : MonoBehaviour
{
    [Header("Look Settings")]
    [Range(0, 100)] public float mouseSensitivity = 25f;
    [Range(0f, 200f)] private float snappiness = 100f;

    [Header("Speeds")]
    [Range(0f, 20f)] public float walkSpeed = 5f;
    [Range(0f, 30f)] public float sprintSpeed = 10f;
    [Range(0f, 10f)] public float crouchSpeed = 3f;
    public float slideSpeed = 9f;
    [Range(0f, 15f)] public float jumpSpeed = 3f;

    [Header("Crouch / Slide / FOV")]
    public float crouchHeight = 1f;
    public float crouchCameraHeight = 0.5f;
    public float slideDuration = 0.7f;
    public float slideFovBoost = 5f;
    public float slideTiltAngle = 5f;
    public float normalFov = 60f;
    public float sprintFov = 70f;
    public float fovChangeSpeed = 5f;

    [Header("Gravity & Times")]
    [Range(0f, 50f)] public float gravity = 9.81f;
    public bool coyoteTimeEnabled = true;
    public float coyoteTimeDuration = 0.25f;

    [Header("Head Bob")]
    public float walkingBobbingSpeed = 9f;
    public float bobbingAmount = 0.01f;
    [Tooltip("Intensidad del tilt lateral al caminar")]
    public float horizontalTiltAmount = 2f;  

    [Header("Capsule & Layers")]
    public Transform groundCheck;
    public float groundDistance = 0.3f;
    public LayerMask groundMask;

    [Header("References")]
    public Transform playerCamera;
    public Transform cameraParent;

    [Header("Toggles")]
    public bool canSlide = true;
    public bool canJump = true;
    public bool canSprint = true;
    public bool canCrouch = true;

    [Header("Movement Smoothing")]
    public float accelTime = 0.1f;
    public float deccelTime = 0.2f;
    public float airControlMultiplier = 2f;
    private Vector3 horizontalVelocity;
    private Vector3 moveVelocityRef;

    [Header("Jump Buffer")]
    public float jumpBufferDuration = 0.1f;
    private float jumpBufferTimer;

    private float rotX, rotY;
    private float xVelocity, yVelocity;
    private CharacterController characterController;
    private Vector3 moveDirection = Vector3.zero;
    private bool isGrounded;
    private Vector2 moveInput;
    public bool isCrouching, isSliding;
    private float slideTimer, postSlideCrouchTimer, coyoteTimer;
    private Vector3 slideDirection;
    private float originalHeight, originalCameraParentHeight;
    private Camera cam;
    private AudioSource slideAudioSource;
    private float bobTimer;
    private Vector3 recoil = Vector3.zero;
    private bool isLook = true, isMove = true;
    private bool isWalking; // ahora accesible globalmente

    public float CurrentCameraHeight =>
        isCrouching || isSliding ? crouchCameraHeight : originalCameraParentHeight;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        cam = playerCamera.GetComponent<Camera>();
        originalHeight = characterController.height;
        originalCameraParentHeight = cameraParent.localPosition.y;
        slideAudioSource = gameObject.AddComponent<AudioSource>();
        slideAudioSource.playOnAwake = false;
        slideAudioSource.loop = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        isGrounded = characterController.isGrounded;

        if (canJump && Input.GetKeyDown(KeyCode.Space))
            jumpBufferTimer = jumpBufferDuration;
        else
            jumpBufferTimer -= Time.deltaTime;

        if (isLook)
        {
            float mX = Input.GetAxis("Mouse X") * 10f * mouseSensitivity * Time.deltaTime;
            float mY = Input.GetAxis("Mouse Y") * 10f * mouseSensitivity * Time.deltaTime;
            rotX += mX; rotY -= mY;
            rotY = Mathf.Clamp(rotY, -90f, 90f);

            xVelocity = Mathf.Lerp(xVelocity, rotX, snappiness * Time.deltaTime);
            yVelocity = Mathf.Lerp(yVelocity, rotY, snappiness * Time.deltaTime);

            playerCamera.localRotation = Quaternion.Euler(yVelocity, 0f, isSliding ? -slideTiltAngle : 0f);
            transform.rotation = Quaternion.Euler(0f, rotX, 0f);
        }

        HandleHeadBob();
        HandleCrouchAndSlideSetup();
        HandleSlide();
        HandleCrouchTransition();
        HandleFov();
        HandleMovement();
    }

    private void HandleMovement()
    {
        moveInput.x = Input.GetAxis("Horizontal");
        moveInput.y = Input.GetAxis("Vertical");

        bool isWalking = canSprint &&
                         Input.GetKey(KeyCode.LeftShift) &&
                         moveInput.y > 0.1f &&
                         isGrounded &&
                         !isCrouching &&
                         !isSliding;

        float speed = isCrouching ? crouchSpeed : (isWalking ? walkSpeed : sprintSpeed);

        if (isGrounded)
        {
            moveDirection.y = -2f;
            coyoteTimer = coyoteTimeEnabled ? coyoteTimeDuration : 0f;

            if (jumpBufferTimer > 0f && canJump)
            {
                moveDirection.y = jumpSpeed;
                jumpBufferTimer = 0f;
                coyoteTimer = 0f;
            }
        }
        else
        {
            if (coyoteTimeEnabled)
                coyoteTimer -= Time.deltaTime;

            if (jumpBufferTimer > 0f && coyoteTimer > 0f && canJump)
            {
                moveDirection.y = jumpSpeed;
                jumpBufferTimer = 0f;
                coyoteTimer = 0f;
            }

            moveDirection.y -= gravity * Time.deltaTime;
        }

        Vector3 dir = new Vector3(moveInput.x, 0f, moveInput.y).normalized;
        bool inAir = !isGrounded && coyoteTimer <= 0f;
        float smoothT = (dir.magnitude > 0.1f ? accelTime : deccelTime) * (inAir ? airControlMultiplier : 1f);
        Vector3 targetV = transform.TransformDirection(dir) * speed;
        horizontalVelocity = Vector3.SmoothDamp(horizontalVelocity, targetV, ref moveVelocityRef, smoothT);

        if (dir.magnitude < 0.1f && isGrounded)
        {
            horizontalVelocity = Vector3.MoveTowards(horizontalVelocity, Vector3.zero, (speed / deccelTime) * Time.deltaTime);
        }

        moveDirection.x = horizontalVelocity.x;
        moveDirection.z = horizontalVelocity.z;

        characterController.Move(moveDirection * Time.deltaTime);
    }

    private void HandleFov()
    {
        bool isWalking = canSprint &&
                         Input.GetKey(KeyCode.LeftShift) &&
                         moveInput.y > 0.1f &&
                         isGrounded &&
                         !isCrouching &&
                         !isSliding;

        float target = isWalking ? normalFov : (isSliding ? sprintFov + slideFovBoost : sprintFov);
        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, target, Time.deltaTime * fovChangeSpeed);
    }

    private void HandleHeadBob()
    {
        if (!isGrounded || isSliding || isCrouching)
        {
            bobTimer = 0f;
            cameraParent.localPosition = new Vector3(
                cameraParent.localPosition.x,
                Mathf.Lerp(cameraParent.localPosition.y, CurrentCameraHeight, Time.deltaTime * walkingBobbingSpeed),
                cameraParent.localPosition.z
            );
            recoil = Vector3.zero;
            cameraParent.localRotation = Quaternion.RotateTowards(cameraParent.localRotation, Quaternion.Euler(recoil), walkingBobbingSpeed * Time.deltaTime);
            return;
        }

        bool moving = Mathf.Abs(characterController.velocity.x) > 0.1f || Mathf.Abs(characterController.velocity.z) > 0.1f;
        if (moving)
        {
            float speedMult = Input.GetKey(KeyCode.LeftShift) ? 1f : 1.2f;
            bobTimer += Time.deltaTime * walkingBobbingSpeed * speedMult;
            cameraParent.localPosition = new Vector3(
                cameraParent.localPosition.x,
                CurrentCameraHeight + Mathf.Sin(bobTimer) * bobbingAmount,
                cameraParent.localPosition.z
            );
            recoil.z = moveInput.x * -2f;
        }
        else
        {
            bobTimer = 0f;
            recoil = Vector3.zero;
            cameraParent.localPosition = new Vector3(
                cameraParent.localPosition.x,
                Mathf.Lerp(cameraParent.localPosition.y, CurrentCameraHeight, Time.deltaTime * walkingBobbingSpeed),
                cameraParent.localPosition.z
            );
        }

        cameraParent.localRotation = Quaternion.RotateTowards(cameraParent.localRotation, Quaternion.Euler(recoil), walkingBobbingSpeed * Time.deltaTime);
    }

    private void HandleCrouchAndSlideSetup()
    {
        bool wantsToCrouch = canCrouch && Input.GetKey(KeyCode.LeftControl) && !isSliding;
        Vector3 p1 = transform.position + characterController.center - Vector3.up * (characterController.height * 0.5f);
        Vector3 p2 = p1 + Vector3.up * characterController.height * 0.6f;
        float capR = characterController.radius * 0.95f;
        float castDist = isSliding ? originalHeight + 0.2f : originalHeight - crouchHeight + 0.2f;
        bool hasCeiling = Physics.CapsuleCast(p1, p2, capR, Vector3.up, castDist, groundMask);

        if (isSliding) postSlideCrouchTimer = 0.1f;

        if (postSlideCrouchTimer > 0f)
        {
            postSlideCrouchTimer -= Time.deltaTime;
            isCrouching = canCrouch;
        }
        else
        {
            isCrouching = canCrouch && (wantsToCrouch || (hasCeiling && !isSliding));
        }

        if (canSlide && !isWalking && Input.GetKeyDown(KeyCode.LeftControl) && isGrounded)
        {
            isSliding = true;
            slideTimer = slideDuration;
            slideDirection = moveInput.magnitude > 0.1f ? (transform.right * moveInput.x + transform.forward * moveInput.y).normalized : transform.forward;
        }
    }

    private void HandleSlide()
    {
        if (!isSliding) return;

        slideTimer -= Time.deltaTime;
        if (slideTimer <= 0f || !isGrounded)
            isSliding = false;

        float t = slideTimer / slideDuration;
        float speed = slideSpeed * Mathf.Lerp(0.5f, 1f, t * t);
        characterController.Move(slideDirection * speed * Time.deltaTime);
    }

    private void HandleCrouchTransition()
    {
        float targetH = isCrouching || isSliding ? crouchHeight : originalHeight;
        characterController.height = Mathf.Lerp(characterController.height, targetH, Time.deltaTime * 15f);
        characterController.center = new Vector3(0f, characterController.height * 0.5f, 0f);
    }

    public void SetControl(bool s) { SetLookControl(s); SetMoveControl(s); }
    public void SetLookControl(bool s) => isLook = s;
    public void SetMoveControl(bool s) => isMove = s;
    public void SetCursorVisibility(bool v)
    {
        Cursor.lockState = v ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = v;
    }
}
