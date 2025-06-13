
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Events;

namespace Player
{
    public class Player : MonoBehaviour
    {
       
        [Header("VectorSpace Method")]
        private Vector3 tempVector;
        [Header("Sliding")]
        public bool slideReady;

        [Header("Camera")]
        public Transform standingCameraHeight;
        public Transform crouchingCameraHeight;



        [Header("Dashing")]
        private float dashLerpCounter;
        public bool isDashing;

        [Header("References")]
        private Rigidbody rb;
        //
        public PlayerController2 playerController;

        public PlayerInput playerInput;

        [SerializeField] private Transform cameraTransform;
        [SerializeField] private Transform sizeArmsY;
        private float sizeArmsYInitial;
        //public enum MovementState { Sprinting, Crouching, Air }
        //public MovementState state;
    
       

        private bool _wasGrounded;
        private bool _isCrouching;
        [SerializeField] private Transform _posStartRotation;
        private void Start()
        {
            rb = GetComponent<Rigidbody>();
            UpdateCameraHeight();
            sizeArmsYInitial = sizeArmsY.localScale.y;
            transform.LookAt(_posStartRotation);

        }

        private void Update()
        {
           // StateHandler();

            CheckGroundedEvents();
        }
        private Vector2 VectorSpace(Vector2 directionVector)
        {
            tempVector = new Vector3(directionVector.x, 0.0f, directionVector.y);
            tempVector = transform.TransformDirection(tempVector);
            tempVector.Normalize();
            return new Vector2(tempVector.x, tempVector.z);
        }
        public void ApplyGravity(float amount)
        {
            rb.AddForce(Physics.gravity * (rb.mass * rb.mass) * amount);
        }
        public void ApplyFriction(float friction)
        {
            rb.AddForce(new Vector3(rb.velocity.x * -friction, 0, rb.velocity.z * -friction));
        }
        public void ApplyVerticalFriction(float friction)
        {
            rb.AddForce(new Vector3(0, rb.velocity.y * -friction, 0));
        }
        public void GroundedJump(float force)
        {
            rb.AddForce(new Vector3(0, force * 10, 0));
        }
        public void AirborneJump(float force, float dirForce, Vector3 dir)
        {
            rb.velocity = new Vector3(rb.velocity.x / 4, 0.0f, rb.velocity.z / 4);
            rb.AddForce(new Vector3(0, force * 10, 0) + dir * dirForce * 10);
        }
        public void WallJump(float upForce, float range, float wallPush, Vector3 dir, Vector3 wallDir)
        {
            rb.AddForce(dir * range * 10 + new Vector3(0, upForce * 10, 0) + wallDir * wallPush * 10);
        }
        public void SlideJump(float force, Vector3 dir)
        {
            rb.AddForce(new Vector3(0, force * 10, 0) + dir * 10);
        }
        public void Walk(Vector2 moveDir, float walkSpeed, float walkSpeedIncrease, ref float rampUpCounter, float rampUpTime)
        {
            if (!playerInput.canMove) return;
            if (rampUpCounter < rampUpTime)
            {
                rampUpCounter += 0.05f;
                if (rampUpCounter > rampUpTime) { rampUpCounter = rampUpTime; } //prevent overshoot
            }

            moveDir = VectorSpace(moveDir);

            rb.AddForce(new Vector3(moveDir.x * (walkSpeed + (walkSpeedIncrease * (rampUpCounter / rampUpTime))), 0, moveDir.y * (walkSpeed + (walkSpeedIncrease * (rampUpCounter / rampUpTime)))));
        }
        public void Slide(float strenght, bool grounded)
        {

            if (slideReady)
            {
                if (grounded) { rb.AddForce(new Vector3(rb.velocity.x, 0, rb.velocity.z).normalized * strenght * 100); }
                else { rb.AddForce(new Vector3(rb.velocity.x, -0.8f, rb.velocity.z).normalized * strenght * 100); }
            }
            slideReady = false;
        }
        public void Wallrun(bool rightSide, float dir, float speed, Vector3 normal)
        {
            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
            if (rightSide)
            {
                rb.AddForce(Vector3.Cross(Vector3.up, normal) * dir * speed);
            }
            else
            {
                rb.AddForce(Vector3.Cross(normal, Vector3.up) * dir * speed);
            }
        }
        public void AirControl(Vector2 moveDir, float airSpeed)
        {
            moveDir = VectorSpace(moveDir);
            rb.AddForce(new Vector3(moveDir.x * airSpeed, 0, moveDir.y * airSpeed));
        }
        public void Climb(Vector2 moveDir, float climbSpeed)
        {
            rb.AddForce(Vector3.up * climbSpeed * moveDir.y);
        }
        public void Vault(Vector3 targetPos)
        {
            transform.position = targetPos;
        }
        public void Dashhold(Vector2 dir, float speed)
        {

        }
        public IEnumerator Dash(float dashSpeed, Vector3 currPos, Vector3 targetPos)
        {
            rb.velocity = Vector3.zero;
            isDashing = true;
            while (dashLerpCounter <= dashSpeed)
            {
                transform.position = Vector3.Slerp(
                    currPos,
                    targetPos,
                    (dashLerpCounter / dashSpeed));
                dashLerpCounter += Time.deltaTime;

                yield return null;
            }
            transform.position = targetPos;
            dashLerpCounter = 0;
            rb.velocity = new Vector3(rb.velocity.x, 0.0f, rb.velocity.z);
            rb.AddForce((targetPos - currPos).normalized * 400);
            isDashing = false;
            yield return null;
        }








        public void Crounch(bool b)
        {
           

            if (b)
            {
                Vector3 scale = sizeArmsY.localScale;  
                scale.y = 1.45f;                          
                sizeArmsY.localScale = scale;         
                CheckCrouchEvents(true);
            }
            else
            {
                Vector3 scale = sizeArmsY.localScale;
                scale.y = sizeArmsYInitial; 
                sizeArmsY.localScale = scale;
                CheckCrouchEvents(false);
            }
            HandleCrouchTransition();
        }

        

        void HandleCrouchTransition()
        {
            // Si querés hacer animaciones o escalar al personaje, lo hacés acá
            // Por ejemplo, podrías escalar su Y con Lerp para hacer que se agache visualmente
            Vector3 targetScale = playerController.isCrouching ? new Vector3(1, 0.5f, 1) : Vector3.one;
            transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * 10f);
        }

        void UpdateCameraHeight()
        {
            Vector3 targetPos = playerController.isCrouching ? crouchingCameraHeight.localPosition : standingCameraHeight.localPosition;
            cameraTransform.localPosition = Vector3.Lerp(cameraTransform.localPosition, targetPos, Time.deltaTime);
        }
        private bool IsGrounded()
        {
            float distanceToGround = 1f;
            return Physics.Raycast(transform.position, Vector3.down, distanceToGround + 0.1f);
        }

        //private void StateHandler()
        //{
        //    if (Input.GetKey(KeyCode.LeftControl))
        //    {
        //        playerController.lastStatus = playerController.status;
        //        playerController.status = Status.Crouching;
        //    }
        //    else if (IsGrounded())
        //    {
        //        playerController.lastStatus = playerController.status;
        //        playerController.status = Status.Sprinting;
        //    }
        //    else
        //    {
        //        playerController.lastStatus = playerController.status;
        //        playerController.status = Status.Air;
        //    }
        //}

        public void ResetJump()
        {
            playerController._readyToJump = true;
            EventManager.TriggerEvent("OnIdle", gameObject);
        }

        private void CheckGroundedEvents()
        {
            bool grounded = IsGrounded();
            if (grounded && !_wasGrounded)
                EventManager.TriggerEvent("OnLand", gameObject);

            _wasGrounded = grounded;
        }

        //private void CheckCrouchEvents()
        //{
        //    bool currentlyCrouching = playerController.status == Status.Crouching;

        //    if (currentlyCrouching && !_isCrouching)
        //        EventManager.TriggerEvent("OnCrouchEnter", gameObject);
        //    else if (!currentlyCrouching && _isCrouching)
        //        EventManager.TriggerEvent("OnCrouchExit", gameObject);

        //    _isCrouching = currentlyCrouching;
        //}
        private void CheckCrouchEvents(bool b)
        {
            if (b)
            {
                EventManager.TriggerEvent("OnCrouchEnter", gameObject);
            }
            else
            {
                EventManager.TriggerEvent("OnCrouchExit", gameObject);
            }
        }
    }
}
