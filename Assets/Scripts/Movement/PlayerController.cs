//All tutorial from:  https://www.bilibili.com/video/BV1dP4y1574a?p=3

using General;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace Movement
{
    public class PlayerController : SingletonMonoBehavior<PlayerController>
    {
        //Player information.
        private MyInputActions _inputActions;
        private Rigidbody _rigidbody;
        public Transform playerModel;
        

        [FormerlySerializedAs("_moveDir")] [SerializeField]private Vector3 moveDir = Vector3.zero;

        [FormerlySerializedAs("_aimDir")] [SerializeField]private Vector3 aimDir = Vector3.up;
        // Start is called before the first frame update
        private void Start()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _inputActions = new MyInputActions();
            _inputActions.Enable();
            
            //Move
            _inputActions.Player.Move.started += Player_Move_Started;
            _inputActions.Player.Move.performed += Player_Move_Performed;
            _inputActions.Player.Move.canceled += Player_Move_Canceled;
            
            //Aim
            _inputActions.Player.Aim.started += Player_Aim_Started;
            _inputActions.Player.Aim.performed += Player_Aim_Performed;
            _inputActions.Player.Aim.canceled += Player_Aim_Canceled;
        }


        private void FixedUpdate()
        {
            //aiming
            var angle = Mathf.Atan2(aimDir.y, aimDir.x) * (180/Mathf.PI);
            playerModel.rotation = Quaternion.Euler(new Vector3(0,0,0));
            //moving
            _rigidbody.AddForce(moveDir.normalized * 10);
        }

        #region Player Moving

        private void Player_Move_Started(InputAction.CallbackContext obj)
        {
            Debug.Log("Started");
        }
        private void Player_Move_Performed(InputAction.CallbackContext obj) 
        { 
            Debug.Log("Performed"); 
            moveDir = obj.ReadValue<Vector2>();
        }
        private void Player_Move_Canceled(InputAction.CallbackContext obj) 
        { 
            Debug.Log("Canceled"); 
            moveDir = Vector3.zero;
        }

        #endregion


        #region Aiming

        private void Player_Aim_Started(InputAction.CallbackContext obj)
        {
            Debug.Log("Aim Started");
            aimDir = Vector3.up;
        }
        private void Player_Aim_Performed(InputAction.CallbackContext obj)
        {
            Debug.Log("Aim Performed");
            aimDir = obj.ReadValue<Vector2>();
        }
        private void Player_Aim_Canceled(InputAction.CallbackContext obj)
        {
            Debug.Log("Aim Canceled");
            aimDir = Vector3.up;
        }

        #endregion
        
        
        
        
    }
}
