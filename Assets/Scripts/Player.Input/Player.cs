using General;
using UnityEngine;
using UnityEngine.Serialization;

namespace Player.Input
{
    public class Player : SingletonMonoBehavior<Player>
    {
        private CharacterController _controller;
        private Vector3 _playerVelocity;

        [FormerlySerializedAs("_playerSpeed")] [SerializeField]
        private float playerSpeed = 2.0f;

        public bool isAttacking;

        private PlayerInputActions _playerInput;

        [SerializeField] private Animator _animator;

        public override void Awake()
        {
            _playerInput = new PlayerInputActions();
        }

        private void OnEnable()
        {
            _playerInput.Enable();
        }

        private void OnDisable()
        {
            _playerInput.Disable();
        }

        private void Start()
        {
            _controller = GetComponent<CharacterController>();
        }

        private void Update()
        {

            var movementInput = _playerInput.Player1.Movement.ReadValue<Vector2>();
            var move = new Vector3(movementInput.x, 0f, movementInput.y);
            _controller.Move(move * Time.deltaTime * playerSpeed);
            if (move != Vector3.zero)
            {
                gameObject.transform.forward = move;
                _animator.SetBool("isRunning",true);
            }
            else
            {
                _animator.SetBool("isRunning",false);
            }

            //Attacking the other player
            if (_playerInput.Player1.Use.triggered && !isAttacking)
            {
                isAttacking = true;
                _animator.SetBool("isPunching",true);
            }
        }



       #region ControlSystems
        
       /*  private bool AnimatorIsPlaying(string stateName){
            return AnimatorIsPlaying() && _animator.GetCurrentAnimatorStateInfo(0).IsName(stateName);
        }

        public void AttackingRule()
        {

            if (isAttacking && AnimatorIsPlaying())
            {
                _animator.SetBool("isPunching",true);
            }
        }*/

       

        #endregion
    }
}
