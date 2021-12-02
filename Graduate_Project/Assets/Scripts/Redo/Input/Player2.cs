namespace Redo.Input
{
    public class Player2 : InputDefine
    {
        
        private void Start()
        {
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