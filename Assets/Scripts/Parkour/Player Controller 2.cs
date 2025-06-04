using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Player;
using TMPro.Examples;

public enum Status { idle, Sprinting, Crouching, vaulting, sliding, wallrunning, Air, climbing, dashholding }

public class PlayerController2 : MonoBehaviour
{
    public float currentSpeed;
    public float speed = 5f;
    public Vector3 inputDirection;
    [Header("Status")]
    public Status status;
    public Status lastStatus;

    [Header("Monitoring")]
    public float horizontalSpeed;

    [Header("External Control")]
    public bool isPaused;

    [Header("External Forces")]
    public float friction;
    public float gravity;

    [Header("Walking")]
    public float walkSpeed;
    private float moveRampUpCounter;
    public float moveRampUpTime;
    public float walkSpeedIncrease;

    [Header("Crouching & Sliding")]
    public float crouchSpeed;
    public float slideStrenght;
    public float slideFrictionMod;
    public float slideThreshold;
    public bool isCrouching;


    [Header("Crouch")]
    public float standHeight = 2f;
    public float crouchHeight = 1f;
    public float standCenterY;
    public float crouchCenterY = 0.5f;
    public float eyeOffset = 0.6f;
    [Range(0.01f, 0.2f)] public float crouchSmoothTime = 0.08f;

    public float _crouchTargetHeight = 2f;
    public float _crouchTargetCenterY;
    public float _heightSmoothVelocity;
    public float _centerYSmoothVelocity;





    [Header("Wallrunning")]
    public float wallrunSpeedThreshold;
    public float wallrunSpeed;
    private Vector3 wallrunNormalNew;
    private Vector3 wallrunNormalOld;
    private float wallrunCamLerp;
    [Range(0.0f, 1.0f)]
    public float wallrunCamLerpSpeed;
    [Tooltip("At what camera angle to keep wallrunning")]
    [Range(0.0f, 120.0f)]
    public float wallrunMaxAngle;
    private Vector3 currentCamRotation;
    private float wallrunCooldownTimer;
    [Tooltip("Cooldown in seconds")]
    [Range(0.0f, 10.0f)]
    public float wallrunCooldown;
    private bool wallrunReady;
    private float wallrunGravity;
    private float wallrunTimer;
    [Tooltip("How fast gravity increases")]
    public float wallrunFallSpeed;
    private Vector3 wallrunStartDir;

    [Header("Jumping")]
    public float groundedJumpForce;
    public float slideJumpRange;
    public float wallJumpHeight;
    public float wallJumpRange;
    public float wallJumpPushoff;
    public int airJumpCharge;
    public int airJumpChargeAvailable;
    public float airJumpChargeCooldown;
    private float airJumpChargeCooldownTimer;
    public float airborneJumpControl;
    public float airborneJumpForce;

    public bool _readyToJump = true;

    [Header("Air Control")]
    public float airFrictionMod;
    public float airControlSpeed;

    [Header("Climbing")]
    public float climbApproachDistance;
    public float maxClimbApproachAngle;
    public float maxClimbWallAngle;
    public float minClimbWallAngle;
    public float climbSpeed;
    private float climbTimer;
    [Tooltip("How fast gravity increases")]
    public float climbHeight;
    private float climbGravity;
    public float climbFriction;

    [Header("Vaulting")]
    public float vaultApproachDistance;
    public float maxVaultApproachAngle;
    public float maxVaultWallAngle;
    public float minVaultWallAngle;
    public float vaultDistance;
    public float vaultHeight;
    public float vaultCamLerpSpeed;

    [Header("Dashing")]
    public float dashSlowMoFactor;
    public float dashSpeed;
    public float dashDist;
    public float dashHoldMoveSpeed;
    public float dashCooldown;
    [HideInInspector]
    public float dashCooldownTimer;
    public GameObject dashTargetMarkerAir;
    public GameObject dashTargetMarkerWall;


    [Header("References")]
    public PlayerInput input;
    private Player.Player movement;
    [SerializeField]private GroundDetector groundDetector;
    [SerializeField] private CeilingDetector ceilingDetector;
    [SerializeField] private WallrunDetector wallrunDetector;
    [SerializeField] private FrontDetector frontDetector;
    [SerializeField] private CapsuleCollider coll;
    [SerializeField] private Rigidbody rb;
    private CameraController2 camController;
    private Camera cam;

    public bool wallrunLerpTargetGetOnceTrigger = false;

    private Player.Player _player;

    void Start()
    {
        //References
        input = GetComponent<PlayerInput>();
        movement = GetComponent<Player.Player>();
        groundDetector = GetComponentInChildren<GroundDetector>();
        ceilingDetector = GetComponentInChildren<CeilingDetector>();
        wallrunDetector = GetComponentInChildren<WallrunDetector>();
        frontDetector = GetComponentInChildren<FrontDetector>();
        coll = GetComponent<CapsuleCollider>();
        rb = GetComponent<Rigidbody>();
        camController = GetComponent<CameraController2>();
        cam = Camera.main;
        _player = GetComponent<Player.Player>();
        status = Status.idle;

        InitialAbilityCharge();
    }
    void Update()
    {
        if (!isPaused)
        {
            DetermineMonitorables();
            RechargeAbilities();

            if (input.ReleasedDash() || status != Status.Air)
            {
                Time.timeScale = 1.0f;
                Time.fixedDeltaTime = Time.timeScale * 0.02f;
            }

            CamRotationUnwrap();
            ChooseStatus();
            FrictionToSlope();
            RunStatus();
            OnStatusChange();
        }
        if (Input.GetKeyDown(KeyCode.M))
        {
            Metodo();
        }
    }
    void FixedUpdate()
    {
        if (!isPaused)
        {
            RunStatusPhysics();
        }
    }
    void RunStatus()
    {
        switch (status)
        {
            case Status.idle:
                transform.localScale = new Vector3(1, 1, 1);
                movement.slideReady = true;
                if (input.PressedJump())
                {
                    movement.GroundedJump(groundedJumpForce);
                }
                break;
            case Status.Sprinting:
                transform.localScale = new Vector3(1, 1, 1);
                movement.slideReady = true;
                if (input.PressedJump())
                {
                    movement.GroundedJump(groundedJumpForce);
                }
                break;
            case Status.Crouching:
                transform.localScale = new Vector3(1, 0.7f, 1);
                if (input.PressedJump())
                {
                    movement.GroundedJump(groundedJumpForce);
                }
                break;
            case Status.sliding:
                transform.localScale = new Vector3(1, 0.5f, 1);
                if (input.PressedJump() && groundDetector.isGrounded)
                {
                    movement.SlideJump(groundedJumpForce, rb.velocity * slideJumpRange);
                }
                break;
            case Status.wallrunning:
                Debug.Log("wallrunning");
                wallrunTimer += Time.deltaTime;
                wallrunGravity = (wallrunTimer * wallrunTimer) * wallrunFallSpeed;

                if (wallrunDetector.contactR)
                {
                    wallrunCamLerp += Time.deltaTime / wallrunCamLerpSpeed;
                    wallrunNormalNew = Vector3.Cross(Vector3.up, wallrunDetector.wallNormal);
                    if (wallrunNormalNew != wallrunNormalOld)
                    {
                        wallrunCamLerp = 0.0f;
                        wallrunLerpTargetGetOnceTrigger = false;
                    }
                    if (!wallrunLerpTargetGetOnceTrigger)
                    {
                        wallrunLerpTargetGetOnceTrigger = true;
                        wallrunStartDir = transform.forward;
                    }
                    transform.forward = Vector3.Slerp(wallrunStartDir, wallrunNormalNew, wallrunCamLerp);
                    wallrunNormalOld = wallrunNormalNew;
                }
                else if (wallrunDetector.contactL)
                {
                    wallrunCamLerp += Time.deltaTime / wallrunCamLerpSpeed;
                    wallrunNormalNew = Vector3.Cross(wallrunDetector.wallNormal, Vector3.up);
                    if (wallrunNormalNew != wallrunNormalOld)
                    {
                        wallrunCamLerp = 0.0f;
                        wallrunLerpTargetGetOnceTrigger = false;
                    }
                    if (!wallrunLerpTargetGetOnceTrigger)
                    {
                        wallrunLerpTargetGetOnceTrigger = true;
                        wallrunStartDir = transform.forward;
                    }
                    transform.forward = Vector3.Slerp(wallrunStartDir, wallrunNormalNew, wallrunCamLerp);
                    wallrunNormalOld = wallrunNormalNew;
                }
                else if (!wallrunDetector.contactL && !wallrunDetector.contactR)
                {
                    wallrunNormalNew = Vector3.zero;
                    wallrunNormalOld = Vector3.zero;
                    wallrunCamLerp = 0.0f;
                }
                if (input.PressedJump())
                {
                    wallrunGravity = 0.0f;
                    wallrunTimer = 0.0f;
                    rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
                    movement.WallJump(wallJumpHeight, wallJumpRange, wallJumpPushoff, new Vector3(cam.transform.forward.x, 0, cam.transform.forward.z), wallrunDetector.contactR ? -transform.right : transform.right);
                }
                break;
            case (Status.climbing):
                climbTimer += Time.deltaTime;
                climbGravity = (climbTimer * climbTimer) * climbHeight;
                climbGravity = Mathf.Clamp(climbGravity, 0.0f, 9.5f);
                break;
            case (Status.Air):
                transform.localScale = new Vector3(1, 1, 1);
                if (input.PressedJump() && airJumpChargeAvailable > 0)
                {
                    airJumpChargeAvailable--;
                    movement.AirborneJump(airborneJumpForce, airborneJumpControl, cam.transform.forward);
                }
                if (input.ReleasedDash() && dashCooldownTimer >= dashCooldown)
                {
                    dashCooldownTimer = 0.0f;
                    Vector3 dashTarget;
                    RaycastHit hit;
                    if (Physics.Raycast(transform.position, cam.transform.forward, out hit, dashDist))
                    {
                        dashTarget = transform.position + cam.transform.forward * (hit.distance - 0.5f);
                    }
                    else
                    {
                        dashTarget = transform.position + cam.transform.forward * dashDist;
                    }
                    StartCoroutine(movement.Dash(dashSpeed, transform.position, dashTarget));
                }
                break;
            case (Status.dashholding):
                Time.timeScale = dashSlowMoFactor;
                Time.fixedDeltaTime = Time.timeScale * 0.02f;

                Vector3 dashTargetMark;
                RaycastHit hitt;
                if (Physics.Raycast(transform.position, cam.transform.forward, out hitt, dashDist))
                {
                    dashTargetMark = transform.position + cam.transform.forward * (hitt.distance - 0.5f);
                    Instantiate(dashTargetMarkerWall, dashTargetMark, Quaternion.identity);
                }
                else
                {
                    dashTargetMark = transform.position + cam.transform.forward * dashDist;
                    Instantiate(dashTargetMarkerAir, dashTargetMark, Quaternion.identity);
                }
                break;
        }
    }
    void RunStatusPhysics()
    {
        switch (status)
        {
            case Status.idle:
                movement.ApplyFriction(friction);
                movement.ApplyGravity(gravity);
                break;
            case Status.Sprinting:
                movement.ApplyFriction(friction);
                currentSpeed = walkSpeed;
                movement.Walk(input.InputDir(), currentSpeed, walkSpeedIncrease, ref moveRampUpCounter, moveRampUpTime);
                movement.ApplyGravity(gravity);
                break;
            case Status.Crouching:
                movement.ApplyFriction(friction);
               // _player.Crounch();
                currentSpeed = crouchSpeed;
                movement.Walk(input.InputDir(), crouchSpeed, 0, ref moveRampUpCounter, moveRampUpTime);
                movement.ApplyGravity(gravity);
                break;
            case Status.sliding:
                movement.ApplyFriction(friction / slideFrictionMod);
                movement.Slide(groundDetector.isGrounded ? slideStrenght : slideStrenght * 2, groundDetector.isGrounded);
                movement.ApplyGravity(gravity);
                break;
            case Status.wallrunning:
                movement.ApplyFriction(friction);
                movement.Wallrun(wallrunDetector.contactR, input.moveInputDir.y, wallrunSpeed, wallrunDetector.wallNormal);
                movement.ApplyGravity(wallrunGravity);
                break;
            case Status.Air:
                movement.ApplyFriction(friction / airFrictionMod);
                movement.AirControl(input.InputDir(), airControlSpeed);
                movement.ApplyGravity(gravity);
                break;
            case Status.climbing:
                movement.ApplyFriction(friction);
                movement.Climb(input.InputDir(), climbSpeed);
                movement.ApplyGravity(climbGravity);
                movement.ApplyVerticalFriction(climbFriction);
                break;
            case Status.dashholding:
                movement.ApplyFriction(friction / airFrictionMod);
                movement.AirControl(input.InputDir(), airControlSpeed);
                movement.ApplyGravity(gravity);
                movement.Dashhold(input.InputDir(), dashHoldMoveSpeed);
                break;
        }
    }
    public void Metodo()
    {
        Debug.Log(input.InputDir().y > 0);
        Debug.Log(groundDetector.distToGround >= 0.5f);
        Debug.Log((wallrunDetector.contactR || wallrunDetector.contactL));
        Debug.Log((currentCamRotation.y >= -wallrunMaxAngle && currentCamRotation.y <= wallrunMaxAngle));
        Debug.Log(new Vector3(rb.velocity.x, 0, rb.velocity.z).magnitude > wallrunSpeedThreshold);
        Debug.Log("Velocidad " + new Vector3(rb.velocity.x, 0, rb.velocity.z).magnitude);
        Debug.Log("wallrunSpeedThreshold " + wallrunSpeedThreshold);
        Debug.Log(rb.velocity.y < 2.1);
        Debug.Log(wallrunReady && !movement.isDashing);
    }
    void ChooseStatus()
    {
        if (ceilingDetector.canStand && !groundDetector.isGrounded)
        {
            ChangeStatus(Status.Air);
        }
        if (ceilingDetector.canStand && groundDetector.isGrounded && input.InputDir() == Vector2.zero && !movement.isDashing)
        {
            _player.ResetJump();
            ChangeStatus(Status.idle);
        }
        if (ceilingDetector.canStand && groundDetector.isGrounded && input.InputDir() != Vector2.zero && !movement.isDashing)
        {

            ChangeStatus(Status.Sprinting);
        }
        if (input.PressedCrouch() && new Vector3(rb.velocity.x, 0, rb.velocity.z).magnitude < slideThreshold && !movement.isDashing)
        {
            ChangeStatus(Status.Crouching);
        }
        if (input.PressedCrouch() && new Vector3(rb.velocity.x, 0, rb.velocity.z).magnitude > slideThreshold && !movement.isDashing)
        {
            ChangeStatus(Status.sliding);
        }
        if (input.InputDir().y > 0 && groundDetector.distToGround >= 0.5f && (wallrunDetector.contactR || wallrunDetector.contactL) && (currentCamRotation.y >= -wallrunMaxAngle && currentCamRotation.y <= wallrunMaxAngle) && new Vector3(rb.velocity.x, 0, rb.velocity.z).magnitude > wallrunSpeedThreshold && rb.velocity.y < 2.1 && wallrunReady && !movement.isDashing)
        {
            Debug.Log("Change");
            ChangeStatus(Status.wallrunning);
        }
        if (input.InputDir().y > 0 && frontDetector.angleToPlayer < maxClimbApproachAngle && frontDetector.wallAngle > minClimbWallAngle && frontDetector.wallAngle < maxClimbWallAngle && frontDetector.obstacleHeightFromPlayer > 1.2 && frontDetector.distanceToObstacle < climbApproachDistance && rb.velocity.y > -0.25f && status != Status.sliding && !movement.isDashing)
        {
            ChangeStatus(Status.climbing);
        }
        if (ceilingDetector.distToCeiling > 2 && input.InputDir().y > 0 && frontDetector.obstacleDetected && frontDetector.angleToPlayer < maxVaultApproachAngle && frontDetector.wallAngle > minVaultWallAngle && frontDetector.wallAngle < maxVaultWallAngle && frontDetector.obstacleHeightFromPlayer <= vaultHeight && frontDetector.distanceToObstacle < vaultApproachDistance && !movement.isDashing)
        {
            ChangeStatus(Status.vaulting);
        }
        if ((status == Status.Air || status == Status.dashholding) && input.HoldDash() && !movement.isDashing && dashCooldownTimer >= dashCooldown)
        {
            ChangeStatus(Status.dashholding);
        }
    }
    void FrictionToSlope() //Adjusts friction to slope angle in order to not slide off
    {
        if (status == Status.sliding)
        {
            coll.sharedMaterial.dynamicFriction = 0;
            coll.sharedMaterial.staticFriction = 0;
            coll.sharedMaterial.frictionCombine = PhysicMaterialCombine.Minimum;
        }
        else if (groundDetector.groundAngle > 10.0f && input.moveInputDir == Vector2.zero)
        {
            coll.sharedMaterial.dynamicFriction = groundDetector.groundAngle / 16.0f;
            coll.sharedMaterial.staticFriction = groundDetector.groundAngle / 16.0f;
            coll.sharedMaterial.frictionCombine = PhysicMaterialCombine.Maximum;
        }
        else if (true)
        {
            coll.sharedMaterial.dynamicFriction = 0.0f;
            coll.sharedMaterial.staticFriction = 0.0f;
            coll.sharedMaterial.frictionCombine = PhysicMaterialCombine.Minimum;
        }
    }
    void OnStatusChange()
    {
        if (status != lastStatus)
        {
            if (lastStatus == Status.wallrunning) //On every change from wallrunning
            {
                cam.transform.parent = null;
                this.transform.forward = new Vector3(cam.transform.forward.x, this.transform.forward.y, cam.transform.forward.z).normalized;
                cam.transform.parent = this.transform;
                StartCoroutine(camController.CamTiltDefault(cam.transform.localRotation));
                wallrunReady = false;
                wallrunCooldownTimer = 0.0f;
                wallrunCamLerp = 0.0f;
                wallrunLerpTargetGetOnceTrigger = false;
            }
            if (status == Status.wallrunning) //On every change to wallrunning
            {
                StartCoroutine(camController.WallrunCamTilt(cam.transform.localRotation, wallrunDetector.contactR));
                wallrunTimer = 0.0f;
            }
            if (lastStatus == Status.Sprinting && status == Status.idle) //On change from walking to idle
            {
                moveRampUpCounter = 0.0f;
            }
            if (status == Status.climbing) //On every change to climbing
            {
                climbTimer = 0.0f;
            }
            if (status == Status.vaulting) //On every change to climbing
            {
                Vector3 vaultMoveVector = frontDetector.pointOnObstacle + transform.forward * vaultDistance - frontDetector.playerLowPoint.transform.position;
                Vector3 vaultLandingPoint = transform.position + vaultMoveVector;
                Vector3 vaultCamLandingPoint = cam.transform.position + vaultMoveVector;
                StartCoroutine(camController.VaultCamLerp(cam.transform.position, vaultCamLandingPoint, vaultCamLerpSpeed, vaultLandingPoint));
            }
            if (status == Status.sliding) //On every change to sliding
            {
                transform.position = new Vector3(transform.position.x, transform.position.y - 0.5f, transform.position.z);
            }
            if (lastStatus == Status.sliding && status == Status.Crouching) //On cange from sliding to crouching
            {
                transform.position = new Vector3(transform.position.x, transform.position.y + 0.2f, transform.position.z);
            }
            else if (lastStatus != Status.sliding && status == Status.Crouching) //On every change to crouching except from sliding
            {
                transform.position = new Vector3(transform.position.x, transform.position.y - 0.3f, transform.position.z);
            }
        }


        lastStatus = status;
    }
    void CamRotationUnwrap()
    {
        currentCamRotation = cam.transform.localRotation.eulerAngles;
        if (currentCamRotation.y > 180) currentCamRotation.y = currentCamRotation.y - 360;
    }

    //Manual Methods
    void ChangeStatus(Status s)//Used to change status
    {
        if (status == s) { return; }
        status = s;
        if (s == Status.Crouching)
        {
            _crouchTargetHeight = crouchHeight;
            _crouchTargetCenterY = crouchCenterY;
        }
        else
        {
            _crouchTargetHeight = standHeight;
            _crouchTargetCenterY = standCenterY;
        }
        return;
    }
    void RechargeAbilities()
    {
        //Dashing
        if (status != Status.dashholding && !movement.isDashing)
        {
            if (dashCooldownTimer <= dashCooldown) dashCooldownTimer += Time.deltaTime;
            else dashCooldownTimer = dashCooldown;
        }
        //AirJumping
        if (status != Status.Air && status != Status.dashholding && !movement.isDashing)
        {
            if (airJumpChargeAvailable < airJumpCharge)
            {
                if (airJumpChargeCooldownTimer <= airJumpChargeCooldown) airJumpChargeCooldownTimer += Time.deltaTime;
                else
                {
                    airJumpChargeCooldownTimer = 0.0f;
                    airJumpChargeAvailable++;
                }

            }

        }
        //Wallrunning
        if (wallrunCooldownTimer >= wallrunCooldown) { wallrunReady = true; wallrunCooldownTimer = wallrunCooldown; }
        if (!wallrunReady) { wallrunCooldownTimer += Time.deltaTime; }
    }
    void InitialAbilityCharge()
    {
        dashCooldownTimer = dashCooldown;
        airJumpChargeAvailable = airJumpCharge;
    }
    void DetermineMonitorables()
    {
        horizontalSpeed = new Vector3(rb.velocity.x, 0, rb.velocity.z).magnitude;
    }
}
