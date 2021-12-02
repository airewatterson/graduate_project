using General;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Redo.Input
{
    public class InputDefine : SingletonMonoBehavior<InputDefine>
    {
        #region Definer

        //導入InputSystem代碼
        //自Old搬移，並分開成Rigidbody與Collider重寫
        internal PlayerInputActions PlayerInputActions;
        [SerializeField] protected new Rigidbody rigidbody;

        //Vector2轉Vector3的指定數值，由inputsystem自動轉換
        private Vector2 _currentMovementInput;
        internal Vector3 CurrentMovement;
        private bool _isMovementPressed;

        //玩家移動數值
        [SerializeField] internal float movement;

        #endregion
        
        

        protected void OnMovementInput(InputAction.CallbackContext ctx)
        {
            _currentMovementInput = ctx.ReadValue<Vector2>();
            CurrentMovement.x = _currentMovementInput.x;
            CurrentMovement.y = _currentMovementInput.y;
            _isMovementPressed = _currentMovementInput.x != 0 || _currentMovementInput.y != 0;
        }

    }
}