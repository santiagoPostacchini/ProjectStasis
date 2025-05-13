using System;
using UnityEngine;

public partial class FirstPersonController : MonoBehaviour
{
    [Range(0, 100)] public float mouseSensitivity = 25f;
    [Range(0f, 200f)] private float snappiness = 100f;
    [Range(0f, 20f)] public float walkSpeed = 10f;
    [Range(0f, 30f)] public float sprintSpeed = 15f;
    [Range(0f, 10f)] public float crouchSpeed = 6f;
    public float crouchHeight = 1f;
    public float crouchCameraHeight = 0.5f;
    public float slideSpeed = 9f;
    public float slideDuration = 0.7f;
    public float slideFovBoost = 5f;
    public float slideTiltAngle = 5f;
    [Range(0f, 15f)] public float jumpSpeed = 3f;
    [Range(0f, 50f)] public float gravity = 9.81f;
    public bool coyoteTimeEnabled = true;
    public float coyoteTimeDuration = 0.25f;
    public float normalFov = 60f;
    public float sprintFov = 70f;
    public float fovChangeSpeed = 5f;
    public float walkingBobbingSpeed = 14f;
    public float bobbingAmount = 0.05f;
    private float sprintBobMultiplier = 1.2f;
    private float recoilReturnSpeed = 8f;
    public bool canSlide = true;
    public bool canJump = true;
    public bool canSprint = true;
    public bool canCrouch = true;
    public Transform groundCheck;
    public float groundDistance = 0.3f;
    public LayerMask groundMask;
    public Transform playerCamera;
    public Transform cameraParent;
    private float rotX, rotY;
    private float xVelocity, yVelocity;
    private CharacterController characterController;
    private Vector3 moveDirection = Vector3.zero;
    private bool isGrounded;
    private Vector2 moveInput;
    public bool isSprinting;
    public bool isCrouching;
    public bool isSliding;
    private float slideTimer;
    private float postSlideCrouchTimer;
    private Vector3 slideDirection;
    private float originalHeight;
    private float originalCameraParentHeight;
    private float coyoteTimer;
    private Camera cam;
    private AudioSource slideAudioSource;
    private float bobTimer;
    private float defaultPosY;
    private Vector3 recoil = Vector3.zero;
    private bool isLook = true, isMove = true;

    public float CurrentCameraHeight => isCrouching || isSliding ? crouchCameraHeight : originalCameraParentHeight;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        cam = playerCamera.GetComponent<Camera>();
        originalHeight = characterController.height;
        originalCameraParentHeight = cameraParent.localPosition.y;
        defaultPosY = cameraParent.localPosition.y;
        slideAudioSource = gameObject.AddComponent<AudioSource>();
        slideAudioSource.playOnAwake = false;
        slideAudioSource.loop = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        if (isGrounded && moveDirection.y < 0)
        {
            moveDirection.y = -2f;
            coyoteTimer = coyoteTimeEnabled ? coyoteTimeDuration : 0f;
        }
        else if (coyoteTimeEnabled)
        {
            coyoteTimer -= Time.deltaTime;
        }



        if (isLook)
        {
            float mouseX = Input.GetAxis("Mouse X") * 10 * mouseSensitivity * Time.deltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * 10 * mouseSensitivity * Time.deltaTime;

            rotX += mouseX;
            rotY -= mouseY;
            rotY = Mathf.Clamp(rotY, -90f, 90f);

            xVelocity = Mathf.Lerp(xVelocity, rotX, snappiness * Time.deltaTime);
            yVelocity = Mathf.Lerp(yVelocity, rotY, snappiness * Time.deltaTime);

            if (isSliding)
            {
                playerCamera.transform.localRotation = Quaternion.Lerp(playerCamera.transform.localRotation, Quaternion.Euler(yVelocity - slideTiltAngle, 0f, 0f), Time.deltaTime * 10f);
            }
            else
            {
                playerCamera.transform.localRotation = Quaternion.Euler(yVelocity, 0f, 0f);
            }
            transform.rotation = Quaternion.Euler(0f, rotX, 0f);
        }

        HandleHeadBob();

        bool wantsToCrouch = canCrouch && Input.GetKey(KeyCode.LeftControl) && !isSliding;
        Vector3 point1 = transform.position + characterController.center - Vector3.up * (characterController.height * 0.5f);
        Vector3 point2 = point1 + Vector3.up * characterController.height * 0.6f;
        float capsuleRadius = characterController.radius * 0.95f;
        float castDistance = isSliding ? originalHeight + 0.2f : originalHeight - crouchHeight + 0.2f;
        bool hasCeiling = Physics.CapsuleCast(point1, point2, capsuleRadius, Vector3.up, castDistance, groundMask);
        Debug.DrawLine(point1, point1 + Vector3.up * castDistance, Color.red);
        Debug.DrawLine(point2, point2 + Vector3.up * castDistance, Color.red);
        if (isSliding)
        {
            postSlideCrouchTimer = 0.1f;
        }
        if (postSlideCrouchTimer > 0)
        {
            postSlideCrouchTimer -= Time.deltaTime;
            isCrouching = canCrouch;
        }
        else
        {
            isCrouching = canCrouch && (wantsToCrouch || (hasCeiling && !isSliding));
        }

        if (canSlide && isSprinting && Input.GetKeyDown(KeyCode.LeftControl) && isGrounded)
        {
            isSliding = true;
            slideTimer = slideDuration;
            slideDirection = moveInput.magnitude > 0.1f ? (transform.right * moveInput.x + transform.forward * moveInput.y).normalized : transform.forward;
        }

        if (isSliding)
        {
            slideTimer -= Time.deltaTime;
            if (slideTimer <= 0f || !isGrounded)
            {
                isSliding = false;
            }
            float slideProgress = slideTimer / slideDuration;
            float currentSlideSpeed = slideSpeed * Mathf.Lerp(0.5f, 1f, slideProgress * slideProgress);
            characterController.Move(slideDirection * currentSlideSpeed * Time.deltaTime);
        }

        float targetHeight = isCrouching || isSliding ? crouchHeight : originalHeight;
        characterController.height = Mathf.Lerp(characterController.height, targetHeight, Time.deltaTime * 15f);
        characterController.center = new Vector3(0f, characterController.height * 0.5f, 0f);

        float targetFov = isSprinting ? sprintFov : (isSliding ? sprintFov + slideFovBoost : normalFov);
        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetFov, Time.deltaTime * fovChangeSpeed);

        HandleMovement();
    }

    private void HandleHeadBob()
    {
        if (!isGrounded || isSliding || isCrouching)
        {
            bobTimer = 0f;
            cameraParent.localPosition = new Vector3(
                cameraParent.localPosition.x,
                Mathf.Lerp(cameraParent.localPosition.y, CurrentCameraHeight, Time.deltaTime * walkingBobbingSpeed),
                cameraParent.localPosition.z);
            recoil = Vector3.zero;
            cameraParent.localRotation = Quaternion.RotateTowards(cameraParent.localRotation, Quaternion.Euler(recoil), recoilReturnSpeed * Time.deltaTime);
            return;
        }

        if (Mathf.Abs(characterController.velocity.x) > 0.1f || Mathf.Abs(characterController.velocity.z) > 0.1f)
        {
            float bobSpeed = walkingBobbingSpeed * (isSprinting ? sprintBobMultiplier : 1f);
            bobTimer += Time.deltaTime * bobSpeed;
            cameraParent.localPosition = new Vector3(
                cameraParent.localPosition.x,
                CurrentCameraHeight + Mathf.Sin(bobTimer) * bobbingAmount,
                cameraParent.localPosition.z);
            recoil.z = moveInput.x * -2f;
        }
        else
        {
            bobTimer = 0f;
            cameraParent.localPosition = new Vector3(
                cameraParent.localPosition.x,
                Mathf.Lerp(cameraParent.localPosition.y, CurrentCameraHeight, Time.deltaTime * walkingBobbingSpeed),
                cameraParent.localPosition.z);
            recoil = Vector3.zero;
        }

        cameraParent.localRotation = Quaternion.RotateTowards(cameraParent.localRotation, Quaternion.Euler(recoil), recoilReturnSpeed * Time.deltaTime);
    }

    private void HandleMovement()
    {
        moveInput.x = Input.GetAxis("Horizontal");
        moveInput.y = Input.GetAxis("Vertical");
        isSprinting = canSprint && Input.GetKey(KeyCode.LeftShift) && moveInput.y > 0.1f && isGrounded && !isCrouching && !isSliding;

        float currentSpeed = isCrouching ? crouchSpeed : (isSprinting ? sprintSpeed : walkSpeed);
        if (!isMove) currentSpeed = 0f;

        Vector3 direction = new Vector3(moveInput.x, 0f, moveInput.y);
        Vector3 moveVector = transform.TransformDirection(direction) * currentSpeed;
        moveVector = Vector3.ClampMagnitude(moveVector, currentSpeed);

        if (isGrounded || coyoteTimer > 0f)
        {
            if (canJump && Input.GetKeyDown(KeyCode.Space) && !isSliding)
            {
                moveDirection.y = jumpSpeed;
            }
            else if (moveDirection.y < 0)
            {
                moveDirection.y = -2f;
            }
        }
        else
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }

        if (!isSliding)
        {
            moveDirection = new Vector3(moveVector.x, moveDirection.y, moveVector.z);
            characterController.Move(moveDirection * Time.deltaTime);
        }
    }

    public void SetControl(bool newState)
    {
        SetLookControl(newState);
        SetMoveControl(newState);
    }

    public void SetLookControl(bool newState)
    {
        isLook = newState;
    }

    public void SetMoveControl(bool newState)
    {
        isMove = newState;
    }

    public void SetCursorVisibility(bool newVisibility)
    {
        Cursor.lockState = newVisibility ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = newVisibility;
    }
}
