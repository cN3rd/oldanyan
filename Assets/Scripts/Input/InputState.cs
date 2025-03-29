using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Input
{
    public class InputState : MonoBehaviour
    {
        private InputSystemActions _actions;

        public Vector2 CurrentMove { get; private set; }
        public Vector2 CurrentLook { get; private set; }

        public bool IsSprinting { get; private set; }

        public void Awake() => _actions = new InputSystemActions();

        public void BlockPlayerInput() => _actions.Player.Disable();
        public void UnblockPlayerInput() => _actions.Player.Enable();

        public void OnEnable()
        {
            _actions.Enable();

            _actions.Player.Move.started += MoveAction;
            _actions.Player.Move.performed += MoveAction;
            _actions.Player.Move.canceled += MoveAction;

            _actions.Player.Look.started += LookAction;
            _actions.Player.Look.performed += LookAction;
            _actions.Player.Look.canceled += LookAction;

            _actions.Player.Sprint.started += SprintAction;
            _actions.Player.Sprint.canceled += SprintAction;

            _actions.Player.Jump.performed += JumpAction;
            _actions.Player.PauseGame.performed += PauseGameAction;
        }

        public void OnDisable()
        {
            _actions.Player.Move.started -= MoveAction;
            _actions.Player.Move.performed -= MoveAction;
            _actions.Player.Move.canceled -= MoveAction;

            _actions.Player.Look.started -= LookAction;
            _actions.Player.Look.performed -= LookAction;
            _actions.Player.Look.canceled -= LookAction;

            _actions.Player.Sprint.started -= SprintAction;
            _actions.Player.Sprint.canceled -= SprintAction;

            _actions.Player.Jump.performed -= JumpAction;
            _actions.Player.PauseGame.performed -= PauseGameAction;

            _actions.Disable();
        }

        public event Action OnJump;
        public event Action OnPauseGame;

        private void JumpAction(InputAction.CallbackContext obj) => OnJump?.Invoke();

        private void PauseGameAction(InputAction.CallbackContext obj) => OnPauseGame?.Invoke();

        private void SprintAction(InputAction.CallbackContext context) => IsSprinting =
            context.phase is InputActionPhase.Started or InputActionPhase.Performed;

        private void LookAction(InputAction.CallbackContext context) =>
            CurrentLook = SafelyReadInput<Vector2>(context);

        private void MoveAction(InputAction.CallbackContext context) =>
            CurrentMove = SafelyReadInput<Vector2>(context);

        private TValue SafelyReadInput<TValue>(InputAction.CallbackContext context)
            where TValue : struct =>
            context.phase == InputActionPhase.Canceled ? default : context.ReadValue<TValue>();
    }
}
