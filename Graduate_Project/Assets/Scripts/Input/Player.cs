using UnityEngine;
using UnityEngine.InputSystem;
using General;
using UnityEngine.Serialization;

//Code written by Aire Watterson.
//Tutorial from https://youtu.be/bXNFxQpp2qk

namespace Input
{
    public class Player : SingletonMonoBehavior<Player>
    {
        //導入InputSystem代碼
        private PlayerInputActions _playerInputActions;
        private CharacterController _characterController;

        //Vector2轉Vector3的指定數值，由inputsystem自動轉換
        private Vector2 _currentMovementInput;
        private Vector3 _currentMovement;
        private bool _isMovementPressed;

        [FormerlySerializedAs("_move")] [SerializeField] private float move;


        public override void Awake()
        {
            _playerInputActions = new PlayerInputActions();
            _characterController = GetComponent<CharacterController>();
            
            //inputsystem名稱.actionmaps名稱.動作.然後要幹嘛...etc(OnMovementInput)
            _playerInputActions.Player1.Movement.started += OnMovementInput;
            _playerInputActions.Player1.Movement.canceled += OnMovementInput;
            _playerInputActions.Player1.Movement.performed += OnMovementInput;
        }

        private void Update()
        {
            _characterController.Move(_currentMovement * move * Time.deltaTime);
        }

        private void OnEnable()
        {
            _playerInputActions.Player1.Enable();
        }

        private void OnDisable()
        {
            _playerInputActions.Player1.Disable();
        }

        private void OnMovementInput(InputAction.CallbackContext ctx)
        {
            _currentMovementInput = ctx.ReadValue<Vector2>();
            _currentMovement.x = _currentMovementInput.x;
            _currentMovement.z = _currentMovementInput.y;
            _isMovementPressed = _currentMovementInput.x != 0 || _currentMovementInput.y != 0;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Item"))
            {
                Debug.Log("TriggerEnter");
            }
        }
    }
    
}
