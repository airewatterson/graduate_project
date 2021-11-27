using General;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Input
{
    public class ForPlayer2 : SingletonMonoBehavior<ForPlayer2>
    {
        //指定控制物件
        [SerializeField] private GameObject itemActive;
        
        //導入InputSystem代碼
        private PlayerInputActions _playerInputActions;
        private CharacterController _characterController;

        //Vector2轉Vector3的指定數值，由inputsystem自動轉換
        private Vector2 _currentMovementInput;
        private Vector3 _currentMovement;
        private bool _isMovementPressed;
        
        //玩家數值
        public int health;
        [SerializeField] private float move;
        

        public override void Awake()
        {
            _playerInputActions = new PlayerInputActions();
            _characterController = GetComponent<CharacterController>();
            
            //inputsystem名稱.actionmaps名稱.動作.然後要幹嘛...etc(OnMovementInput)
            _playerInputActions.Player2.Movement.started += OnMovementInput;
            _playerInputActions.Player2.Movement.canceled += OnMovementInput;
            _playerInputActions.Player2.Movement.performed += OnMovementInput;
            
        }

        private void Update()
        {
            _characterController.Move(_currentMovement * move * Time.deltaTime);
        }

        private void OnEnable()
        {
            _playerInputActions.Player2.Enable();
        }

        private void OnDisable()
        {
            _playerInputActions.Player2.Disable();
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
                //ItemScript.Instance.ApproachItem();
            }
        }

        private void OnTriggerExit(Collider other)
        {
           // ItemScript.Instance.UnApproachItem();
        }



        public void TakeItem(Collider other)
        {
            if (other.CompareTag("Item") && other.name == "TestBomb")
            {
                other.gameObject.SetActive(false);
                itemActive.SetActive(true);
                health -= 1;
            }
            if (health == 0)
            {
                Destroy(this.gameObject);
            }
        }
        
    }
}
