using General;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace Input
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
        private Vector3 _currentMovement;
        [FormerlySerializedAs("_isMovementPressed")] [SerializeField] private bool isMovementPressed;

        //玩家移動數值
        [SerializeField] internal float movement;
        
        //是否正在移動
        private bool _move;

        #endregion
        
        

        protected void OnMovementInput(InputAction.CallbackContext ctx)
        {
            _currentMovementInput = ctx.ReadValue<Vector2>();
            _currentMovement.x = _currentMovementInput.x;
            _currentMovement.z = _currentMovementInput.y;
            isMovementPressed = _currentMovementInput.x != 0 || _currentMovementInput.y != 0;
        }
        
        private void Update()
        {
            _move = false;
            Vector3 movePosition;
            var vector = new Vector3(0, 0, 0);

            if (!isMovementPressed)
            {
                movePosition = vector;
            }
            else
            {
                movePosition = _currentMovement * movement * Time.deltaTime;
            }
            
            transform.Translate(movePosition);
        }

    }
}