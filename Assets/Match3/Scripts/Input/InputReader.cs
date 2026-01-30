using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Match3.Scripts.Input
{
    public interface IInputReader
    {
        event Action<Vector2> OnTouchStarted;
        event Action<Vector2> OnTouchEnded;
        void EnablePlayerAction();
    }

    [CreateAssetMenu(fileName = "InputReader", menuName = "Input/InputReader")]
    public class InputReader : ScriptableObject, InputSystem_Actions.IPlayerActions, IInputReader
    {
        private InputSystem_Actions _inputActions;

        public event Action<Vector2> OnTouchStarted;
        public event Action<Vector2> OnTouchEnded;

        public void EnablePlayerAction()
        {
            if (_inputActions == null)
            {
                _inputActions = new InputSystem_Actions();
                _inputActions.Player.SetCallbacks(this);
            }

            _inputActions.Enable();
        }

        public void OnTouch(InputAction.CallbackContext context)
        {
            if (!context.started) return;
            OnTouchStarted?.Invoke(Touchscreen.current.position.ReadValue());
        }

        public void OnSwipe(InputAction.CallbackContext context)
        {
            if (!context.canceled) return;
            OnTouchEnded?.Invoke(Touchscreen.current.position.ReadValue());
        }

        // Unused
        public void OnMove(InputAction.CallbackContext context) { }
        public void OnLook(InputAction.CallbackContext context) { }
        public void OnAttack(InputAction.CallbackContext context) { }
        public void OnInteract(InputAction.CallbackContext context) { }
        public void OnCrouch(InputAction.CallbackContext context) { }
        public void OnJump(InputAction.CallbackContext context) { }
        public void OnPrevious(InputAction.CallbackContext context) { }
        public void OnNext(InputAction.CallbackContext context) { }
        public void OnSprint(InputAction.CallbackContext context) { }
    }
}

