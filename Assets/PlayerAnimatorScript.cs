using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hypersycos.RogueFrame.Input
{
    public class PlayerAnimatorScript : MonoBehaviour
    {
        private Animator animator;
        private Controller controllerScript;
        private CharacterController characterController;
        private bool waitForJump = false;
        [SerializeField] float runSpeedNormal = 5f;
        // Start is called before the first frame update
        void Start()
        {
            animator = GetComponent<Animator>();
            controllerScript = GetComponent<Controller>();
            characterController = GetComponent<CharacterController>();
        }

        // Update is called once per frame
        void Update()
        {
            Vector3 horizontalVelocity = characterController.velocity;
            horizontalVelocity.y = 0;

            animator.SetFloat("Speed", horizontalVelocity.magnitude / controllerScript.maxSpeed);
            float animatorSpeed = controllerScript.movementModifiers;
            float velocity = horizontalVelocity.magnitude;
            float walkThreshold = Mathf.Min(runSpeedNormal / 2, controllerScript.maxSpeed / 2);
            if (horizontalVelocity != Vector3.zero)
            {
                if (velocity < walkThreshold)
                {
                    animatorSpeed = velocity / (walkThreshold);
                }
                else
                {
                    animatorSpeed = velocity / (runSpeedNormal);
                }
            }
            animator.SetFloat("MotionSpeed", animatorSpeed);
            animator.SetBool("Grounded", controllerScript.IsGrounded());
            if (waitForJump)
            {
                if (characterController.velocity.y > 0)
                {
                    waitForJump = false;
                }
            }
            else
            {
                if (characterController.velocity.y <= 0f)
                {
                    animator.SetBool("Jump", false);
                }
                if (characterController.velocity.y <= -0.5f)
                {
                    animator.SetBool("FreeFall", true);
                }
                else
                {
                    animator.SetBool("FreeFall", false);
                }
            }
        }

        public void Jump()
        {
            waitForJump = true;
            animator.SetBool("Jump", true);
        }
    }
}