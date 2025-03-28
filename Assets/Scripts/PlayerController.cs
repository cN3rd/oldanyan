using Game.Input;
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
        [SerializeField] private InputState inputState;

        [Header("Movement Controls")]
        [SerializeField] private float maxSpeed = 5f;
        [SerializeField] private float acceleration = 15f;
        [SerializeField] private float sprintMultiplier = 2f;
        [SerializeField] private float jumpForce = 5f;

        [Header("Ground checking")]
        [SerializeField] private LayerMask groundLayer;
        [SerializeField] private float groundCheckDistance = 0.1f;

        private Vector3 _horizontalVelocity = Vector3.zero;
        private Vector3 _moveDirection = Vector3.zero;
        private float _pitch, _yaw;

        private void FixedUpdate() => DoMovement();

        private void DoMovement()
        {
            var currentMove = inputState.CurrentMove;
            if (currentMove.sqrMagnitude < MovementThreshold)
            {
                return;
            }

            // Calculate move direction in world space
            _moveDirection.x = cameraTransform.right.x * currentMove.x +
                               cameraTransform.forward.x * currentMove.y;

            _moveDirection.z = cameraTransform.right.z * currentMove.x +
                               cameraTransform.forward.z * currentMove.y;

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

            if (currentSpeedSquared > effectiveMaxSpeedSquared)
            {
                float speedFactor = effectiveMaxSpeed / Mathf.Sqrt(currentSpeedSquared);
                rb.linearVelocity = new Vector3(
                    _horizontalVelocity.x * speedFactor,
                    rb.linearVelocity.y,
                    _horizontalVelocity.z * speedFactor
                );
            }
        }

        private void LateUpdate()
        {
            var currentLook = inputState.CurrentLook;
            if (currentLook.sqrMagnitude < MovementThreshold)
            {
                return;
            }

            _pitch = Mathf.Clamp(_pitch + currentLook.y, -90f, 90f);
            _yaw = (_yaw + currentLook.x) % 360f; // Allow full 360° rotation

            cameraTransform.localRotation = Quaternion.Euler(_pitch, 0, 0);
            transform.localRotation = Quaternion.Euler(0, _yaw, 0);
        }

        private void OnEnable() => inputState.OnJump += DoJump;

        private void OnDisable() => inputState.OnJump -= DoJump;

        private void DoJump()
        {
            var rayStart = transform.position - new Vector3(0, playerCollider.bounds.extents.y, 0);
            if (!Physics.Raycast(rayStart, Vector3.down, groundCheckDistance, groundLayer))
            {
                return;
            }

            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }
}
