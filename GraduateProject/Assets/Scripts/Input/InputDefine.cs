/*tutorial from https://www.youtube.com/watch?v=bXNFxQpp2qk&t=1242s*/


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
        
        #endregion

        [Header("動畫控制項")]
        [SerializeField]private Animator animator;
        private static readonly int IsRunning = Animator.StringToHash("isRunning");
        private float _rotationFactorPerFrame = 1.0f;
        [SerializeField] private GameObject rotateMesh;

        protected void OnMovementInput(InputAction.CallbackContext ctx)
        {
            _currentMovementInput = ctx.ReadValue<Vector2>();
            _currentMovement.x = _currentMovementInput.x;
            _currentMovement.z = _currentMovementInput.y;
            isMovementPressed = _currentMovementInput.x != 0 || _currentMovementInput.y != 0;
        }
        
        private void Update()
        {
            HandleAnimation();
            HandleRotation();
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

        private void HandleAnimation()
        {
            var isRunning = animator.GetBool(IsRunning);

            if (isMovementPressed && !isRunning)
            {
                animator.SetBool(IsRunning,true);
            }
            else if (!isMovementPressed && isRunning)
            {
                animator.SetBool(IsRunning,false);
            }
        }

        private void HandleRotation()
        {
            Vector3 positionToLookAt;
            positionToLookAt.x = _currentMovement.x;
            positionToLookAt.y = 0.0f;
            positionToLookAt.z = _currentMovement.z;
            var currentQuaternion = transform.rotation;

            if (isMovementPressed)
            {
                var targetQuaternion = Quaternion.LookRotation(positionToLookAt);
                rotateMesh.transform.rotation = Quaternion.Slerp(currentQuaternion, targetQuaternion, _rotationFactorPerFrame);
            }
            
            
        }

    }
}