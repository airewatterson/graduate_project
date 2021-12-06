using UnityEngine;

namespace Input
{
    public class Player1 : InputDefine
    {
        // Start is called before the first frame update
        public override void Awake()
        {
            PlayerInputActions = new PlayerInputActions();
            rigidbody = GetComponent<Rigidbody>();
            PlayerInputActions.Player1.Movement.started += OnMovementInput;
            PlayerInputActions.Player1.Movement.canceled += OnMovementInput;
            PlayerInputActions.Player1.Movement.performed += OnMovementInput;
        }
        
        
        private void OnEnable()
        {
            PlayerInputActions.Player1.Enable();
        }

        private void OnDisable()
        {
            PlayerInputActions.Player1.Disable();
        }
    }
}
