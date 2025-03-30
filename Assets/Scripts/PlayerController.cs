using System;
using Game.Input;
using Game.UI;
using UnityEngine;

namespace Game
{
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerController : MonoBehaviour
    {
        private const float MovementThreshold = 0.01f; // Using squared magnitude comparison
        [SerializeField] private Rigidbody rb;
        [SerializeField] private Collider playerCollider;
        [SerializeField] private Transform cameraTransform;
        [SerializeField] private Transform playerPivotTransform;
        [SerializeField] private Animator characterAnimator;
        [SerializeField] private InputState inputState;
        [SerializeField] private Transform handSocket;
        private PickableWand _wand;

        [Header("Movement Controls")]
        [SerializeField] private float maxSpeed = 5f;
        [SerializeField] private float acceleration = 15f;
        [SerializeField] private float deceleration = 25f;
        [SerializeField] private float sprintMultiplier = 2f;
        [SerializeField] private float jumpForce = 5f;

        [Header("Ground checking")]
        [SerializeField] private LayerMask groundLayer;
        [SerializeField] private float groundCheckDistance = 0.1f;

        // Animator IDs
        private static readonly int _speedId = Animator.StringToHash("Speed");
        private static readonly int _jumpId = Animator.StringToHash("Jump");
        private static readonly int _attackId = Animator.StringToHash("Attack");

        private Vector3 _horizontalVelocity = Vector3.zero;
        private Vector3 _moveDirection = Vector3.zero;
        private float _pitch, _yaw;

        private void FixedUpdate() => DoMovement();

        private void LateUpdate()
        {
            DoLook();
            UpdateCameraPosition();
            UpdatePlayerAnimation();
        }

        private void UpdatePlayerAnimation() => characterAnimator.SetFloat(_speedId, rb.linearVelocity.magnitude / (maxSpeed * sprintMultiplier));

        private void OnEnable()
        {
            inputState.OnJump += DoJump;
            inputState.OnAttack += DoAttack;
        }

        private void OnDisable()
        {
            inputState.OnJump -= DoJump;
            inputState.OnAttack -= DoAttack;
        }

        private void DoMovement()
        {
            var currentMove = inputState.CurrentMove;
            if (currentMove.sqrMagnitude < MovementThreshold)
            {
                // Apply deceleration when no input is detected
                _horizontalVelocity.x = rb.linearVelocity.x;
                _horizontalVelocity.z = rb.linearVelocity.z;

                if (_horizontalVelocity.sqrMagnitude > 0.01f)
                {
                    // Calculate deceleration direction (opposite of current movement)
                    Vector3 decelDirection = -_horizontalVelocity.normalized;
                    rb.AddForce(decelDirection * deceleration, ForceMode.Acceleration);
                }
                return;
            }

            // Calculate move direction in world space
            _moveDirection.x = playerPivotTransform.right.x * currentMove.x +
                               playerPivotTransform.forward.x * currentMove.y;

            _moveDirection.z = playerPivotTransform.right.z * currentMove.x +
                               playerPivotTransform.forward.z * currentMove.y;

            _moveDirection.y = 0;

            float inverseMagnitude =
                1f / Mathf.Sqrt(_moveDirection.x * _moveDirection.x +
                                _moveDirection.z * _moveDirection.z);

            _moveDirection.x *= inverseMagnitude;
            _moveDirection.z *= inverseMagnitude;

            // Apply force
            rb.AddForce(_moveDirection * acceleration, ForceMode.Acceleration);

            // Speed limiting
            float effectiveMaxSpeed = inputState.IsSprinting ? maxSpeed * sprintMultiplier : maxSpeed;
            float effectiveMaxSpeedSquared = effectiveMaxSpeed * effectiveMaxSpeed;

            _horizontalVelocity.x = rb.linearVelocity.x;
            _horizontalVelocity.z = rb.linearVelocity.z;

            float currentSpeedSquared = _horizontalVelocity.x * _horizontalVelocity.x +
                                        _horizontalVelocity.z * _horizontalVelocity.z;

            if (currentSpeedSquared <= effectiveMaxSpeedSquared)
            {
                return;
            }

            // Limit speed
            float speedFactor = effectiveMaxSpeed / Mathf.Sqrt(currentSpeedSquared);
            rb.linearVelocity = new Vector3(
                _horizontalVelocity.x * speedFactor,
                rb.linearVelocity.y,
                _horizontalVelocity.z * speedFactor
            );
        }

        private void UpdateCameraPosition()
        {
            playerPivotTransform.GetPositionAndRotation(out Vector3 position, out Quaternion rotation);
            cameraTransform.SetPositionAndRotation(position, rotation);
        }

        private void DoLook()
        {
            var currentLook = inputState.CurrentLook;
            if (currentLook.sqrMagnitude < MovementThreshold)
            {
                return;
            }

            _pitch = Mathf.Clamp(_pitch + currentLook.y, -90f, 90f);
            _yaw = (_yaw + currentLook.x) % 360f; // Allow full 360° rotation

            playerPivotTransform.localRotation = Quaternion.Euler(_pitch, 0, 0);
            transform.localRotation = Quaternion.Euler(0, _yaw, 0);
        }

        private void DoJump()
        {
            var rayStart = transform.position - new Vector3(0, playerCollider.bounds.extents.y, 0);
            if (!Physics.Raycast(rayStart, Vector3.down, groundCheckDistance, groundLayer))
            {
                return;
            }

            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            characterAnimator.SetTrigger(_jumpId);
        }

        public void AttachWand(PickableWand pickableWand)
        {
            // Adhere to socket entirely
            pickableWand.transform.SetParent(handSocket);
            pickableWand.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);

            _wand = pickableWand;
        }

        public void DoAttack()
        {
            Debug.Log("Starting attack...");
            characterAnimator.SetTrigger(_attackId);
        }

        public void EmitAttackParticle()
        {
            Debug.Log("Attacking...");
        }
    }
}
