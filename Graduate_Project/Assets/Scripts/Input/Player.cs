using System;
using UnityEngine;
using UnityEngine.InputSystem;
using General;
using Item;
using UnityEngine.Serialization;

//Code written by Aire Watterson.
//Tutorial from https://youtu.be/bXNFxQpp2qk
//https://youtu.be/2WnAOV7nHW0

namespace Input
{
    public class Player : SingletonMonoBehavior<Player>
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
        [FormerlySerializedAs("_move")] [SerializeField] private float move;
        
        //玩家背包
        
        
        
#pragma warning disable 108,114
        private void Awake()
#pragma warning restore 108,114
        {
            _playerInputActions = new PlayerInputActions();
            _characterController = GetComponent<CharacterController>();

            //inputsystem名稱.actionmaps名稱.動作.然後要幹嘛...etc(OnMovementInput)
            _playerInputActions.Player1.Movement.started += OnMovementInput;
            _playerInputActions.Player1.Movement.canceled += OnMovementInput;
            _playerInputActions.Player1.Movement.performed += OnMovementInput;
            
            //載入物品欄
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


        private void OnTriggerExit(Collider other)
        {
            //ItemScript.Instance.UnApproachItem();
        }



        public void TakeItem(Collider other)
        {
            /*
            if (other.CompareTag("Item") && other.name == "TestBomb")
            {
                other.gameObject.SetActive(false);
                itemActive.SetActive(true);
                health -= 1;
            }
            if (health == 0)
            {
                Destroy(this.gameObject);
            }*/
        }

        /*public void UseItem(GameObject takePoint, bool isTaking)
        {
            if (itemActive && )
            {
                itemActive.SetActive(false);
                isTaking = true;
                takingItem.transform.position = takePoint.transform.position;
            }
        }*/
    }
}
