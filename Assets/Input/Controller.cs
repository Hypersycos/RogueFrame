using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;

namespace Hypersycos.RogueFrame.Input
{
    public class Controller : NetworkBehaviour
    {
        //input fields
        private Controls ControlAsset;
        private InputAction move;

        //movement fields
        private CharacterController characterController;

        [SerializeField] private float gravityMultiplier = 1f;
        private float gravityForce { get { return Physics.gravity.y * gravityMultiplier * (movementModifiers < 1 ? movementModifiers : 1); } }

        [SerializeField] private float movementSpeed = 4f;
        [SerializeField] private float airStrafeRate = 0.4f;
        [SerializeField] private float drag = 0.7f;
        [field: SerializeField] public float movementModifiers { get; private set; } = 1f;
        private Dictionary<string, float> movementModifierTracker = new Dictionary<string, float>();
        [SerializeField] private float smoothing = 0.1f;
        public float maxSpeed { get { return movementSpeed * movementModifiers; } }
        private float moveForce { get { return maxSpeed / (smoothing / (movementModifiers < 1 ? movementModifiers : 1)) * (IsGrounded() ? 1 : airStrafeRate); } }
        [SerializeField] private float overspeedControl = 0.4f;
        [SerializeField] private float overspeedCarry = 0.25f;
        [SerializeField] private Vector3 velocity = Vector3.zero;
        private Vector3 horizontalVelocity { get { return new Vector3(velocity.x, 0, velocity.z); } }

        private bool crouching = false;
        [SerializeField] private float crouchSpeed = 0.4f;
        [SerializeField] private float slideThreshold = 0.5f;
        [SerializeField] private float slideImpulse = 0.5f;

        [SerializeField] private float jumpHeight = 1f;
        private float jumpForce { get { return Mathf.Sqrt(-2f * gravityForce * jumpHeight); } }
        private bool superJumpAvailable = false;
        [SerializeField] private float superJumpHeight = 4f;
        private float superJumpForce { get { return Mathf.Sqrt(-2f * gravityForce * superJumpHeight * (movementModifiers > 1 ? movementModifiers : 1)); } }
        [SerializeField] private int maxJumps = 2;
        private int jumpsAvailable = 0;

        //other
        [SerializeField] Camera playerCamera;
        PlayerAnimatorScript animatorScript;

        public override void OnNetworkSpawn()
        {
            if (!IsLocalPlayer)
            {
                animatorScript.enabled = false;
                enabled = false;
            }
            characterController = GetComponent<CharacterController>();
            ControlAsset = new Controls();
            playerCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
            Cinemachine.CinemachineVirtualCamera cinemachineCam = playerCamera.GetComponent<Cinemachine.CinemachineVirtualCamera>();
            cinemachineCam.Follow = transform.GetChild(0);
            cinemachineCam.LookAt = transform.GetChild(0);
            animatorScript = GetComponent<PlayerAnimatorScript>();

            move = ControlAsset.Player.Move;

            ControlAsset.Player.Jump.started += DoJump;
            ControlAsset.Player.Crouch.started += DoCrouch;
            ControlAsset.Player.Crouch.canceled += DoCrouch;
            ControlAsset.Player.ToggleCrouch.started += DoCrouch;

            ControlAsset.Player.Enable();

            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        private void DoCrouch(InputAction.CallbackContext obj)
        {
            if (obj.action.name == "Toggle Crouch")
            {
                crouching = !crouching;
            }
            else
            {
                if (obj.phase == InputActionPhase.Started && !crouching)
                {
                    crouching = true;
                }
                else if (obj.phase == InputActionPhase.Canceled && crouching)
                {
                    crouching = false;
                }
            }
            if (crouching)
            {
                if (horizontalVelocity.magnitude / maxSpeed > slideThreshold)
                {
                    if (horizontalVelocity.magnitude < maxSpeed * (1 + slideImpulse))
                    {
                        float magnitude = Mathf.Min(maxSpeed * slideImpulse, maxSpeed * (1 + slideImpulse) - horizontalVelocity.magnitude);
                        Vector3 impulse = horizontalVelocity.normalized * magnitude;
                        velocity.x += impulse.x;
                        velocity.z += impulse.z;
                    }
                }
                AddMovementModifier(crouchSpeed, "crouching");
            }
            else
            {
                RemoveMovementModifier("crouching");
            }
        }

        public override void OnNetworkDespawn()
        {
            ControlAsset.Player.Jump.started -= DoJump;
            ControlAsset.Player.Crouch.started -= DoCrouch;
            ControlAsset.Player.Crouch.canceled -= DoCrouch;
            ControlAsset.Player.ToggleCrouch.started -= DoCrouch;
        }

        private void FixedUpdate()
        {
            if (move == null) return;
            Vector3 inputForce = Vector3.zero;
            inputForce += move.ReadValue<Vector2>().x * GetCameraRight(playerCamera) * moveForce * Time.fixedDeltaTime;
            inputForce += move.ReadValue<Vector2>().y * GetCameraForward(playerCamera) * moveForce * Time.fixedDeltaTime;
            float inputCap = move.ReadValue<Vector2>().magnitude * maxSpeed;
            inputForce *= IsGrounded() ? 1 : airStrafeRate;
            if (inputForce.magnitude > float.Epsilon)
            {
                //transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(inputForce, Vector3.up), Time.fixedDeltaTime / smoothing);
                transform.rotation = Quaternion.LookRotation(horizontalVelocity, Vector3.up);
                //transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(horizontalVelocity, Vector3.up), Time.fixedDeltaTime / smoothing);
            }

            if (horizontalVelocity.magnitude > maxSpeed)
            {
                inputForce *= overspeedControl;
                Vector3 dragForce = -horizontalVelocity.normalized * drag * horizontalVelocity.magnitude * Time.fixedDeltaTime;
                float carryComponent = Vector3.Dot(-dragForce.normalized, inputForce);
                if (carryComponent > overspeedCarry * dragForce.magnitude)
                {
                    Vector3 complement = new Vector3(-dragForce.normalized.z, 0, dragForce.normalized.x);
                    float otherMagnitude = Vector3.Dot(complement, inputForce);
                    inputForce = -dragForce * overspeedCarry + complement * otherMagnitude;
                }
                velocity += inputForce + dragForce;
            }
            else
            {
                if (inputForce == Vector3.zero)
                {
                    float force = maxSpeed * Time.fixedDeltaTime / smoothing * (IsGrounded() ? 1 : airStrafeRate);
                    if (force > horizontalVelocity.magnitude)
                    {
                        velocity.x = 0;
                        velocity.z = 0;
                    }
                    else
                    {
                        velocity -= horizontalVelocity.normalized * force;
                    }
                }
                else
                {
                    Vector3 temp = horizontalVelocity + inputForce;
                    if (temp.magnitude > inputCap)
                    {
                        temp = temp.normalized * inputCap;
                    }
                    velocity.x = temp.x;
                    velocity.z = temp.z;
                }
            }
            characterController.Move(velocity * Time.fixedDeltaTime);
            if (IsGrounded() && velocity.y < 0f)
            {
                velocity.y = 0;
                jumpsAvailable = maxJumps;
                superJumpAvailable = true;
            }
            else
            {
                velocity.y += gravityForce * Time.fixedDeltaTime;
            }
        }

        private Vector3 GetCameraForward(Camera playerCamera)
        {
            Vector3 forward = playerCamera.transform.forward;
            forward.y = 0;
            return forward.normalized;
        }

        private Vector3 GetCameraRight(Camera playerCamera)
        {
            Vector3 right = playerCamera.transform.right;
            right.y = 0;
            return right.normalized;
        }

        private void DoJump(InputAction.CallbackContext obj)
        {
            if (jumpsAvailable > 0)
            {
                if (superJumpAvailable && crouching)
                {
                    Vector3 direction = playerCamera.transform.forward;
                    if (direction.y < 0 && IsGrounded())
                    {
                        direction.y = -direction.y;
                    }
                    float horizontalMagnitude = Mathf.Min(horizontalVelocity.magnitude, maxSpeed/crouchSpeed * (1+slideImpulse));
                    float jumpMagnitude = Mathf.Max(superJumpForce, horizontalMagnitude);
                    velocity = direction * jumpMagnitude;
                    superJumpAvailable = false;
                }
                else
                {
                    velocity.y = jumpForce;
                    animatorScript.Jump();
                    Vector3 inputForce = Vector3.zero;
                    inputForce += move.ReadValue<Vector2>().x * GetCameraRight(playerCamera);
                    inputForce += move.ReadValue<Vector2>().y * GetCameraForward(playerCamera);
                    inputForce = inputForce.normalized;
                    float component = (Vector3.Dot(horizontalVelocity.normalized, inputForce) + 3) / 4;
                    float magnitude = horizontalVelocity.magnitude;
                    velocity.x = inputForce.x * component * magnitude;
                    velocity.z = inputForce.z * component * magnitude;
                }
                jumpsAvailable -= 1;
            }
        }

        public bool IsGrounded()
        {
            //return characterController.isGrounded; //?
            Ray ray = new Ray(this.transform.position + Vector3.up * 0.25f, Vector3.down);
            if (Physics.Raycast(ray, out RaycastHit hit, 0.3f))
                return true;
            else
                return false;
        }

        public void AddMovementModifier(float modifier, string name)
        {
            if (movementModifierTracker.ContainsKey(name))
            {
                RemoveMovementModifier(name);
            }
            movementModifiers *= modifier;
            movementModifierTracker.Add(name, modifier);
        }

        public void RemoveMovementModifier(string name)
        {
            if (movementModifierTracker.ContainsKey(name))
            {
                movementModifiers /= movementModifierTracker[name];
                movementModifierTracker.Remove(name);
            }
        }
    }
}