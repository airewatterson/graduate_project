using System;
using UnityEngine;

namespace Input
{
    public class Player1 : InputDefine
    {
        private Animator _animator;
        // Start is called before the first frame update

        private void Start()
        {
            _animator = GetComponentInChildren<Animator>();
        }

        private void FixedUpdate()
        {
            
        }

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
