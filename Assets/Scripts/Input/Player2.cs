using UnityEngine;

namespace Input
{
    public class Player2 : InputDefine
    {
        public override void Awake()
        {
            PlayerInputActions = new PlayerInputActions();
            rigidbody = GetComponent<Rigidbody>();
            PlayerInputActions.Player2.Movement.started += OnMovementInput;
            PlayerInputActions.Player2.Movement.canceled += OnMovementInput;
            PlayerInputActions.Player2.Movement.performed += OnMovementInput;
        }
        
        private void OnEnable()
        {
            PlayerInputActions.Player2.Enable();
        }

        private void OnDisable()
        {
            PlayerInputActions.Player2.Disable();
        }
    }
}