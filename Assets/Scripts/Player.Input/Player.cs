using System;
using Cinemachine;
using DamageSys;
using General;
using Photon.Pun;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;

namespace Player.Input
{
    public class Player : SingletonMonoBehavior<Player>, IDamageable
    {
        [Header("玩家資訊")] 
        public int playerHp = 3;
        
        
        
        private CharacterController _controller;
        private Vector3 _playerVelocity;

        [FormerlySerializedAs("_playerSpeed")] [SerializeField]
        private float playerSpeed = 2.0f;

        public bool isAttacking;

        private PlayerInputActions _playerInput;

        [SerializeField] private Animator _animator;

        //for Photon
        [Header("Photon設定")]
        private PhotonView _photonView;
        [SerializeField] private CinemachineVirtualCamera cam1;
        [SerializeField] private CinemachineVirtualCamera cam2;
        
        
        //keys
        public bool isCollected;
        private int _getKey;
        [SerializeField] private TextMeshProUGUI keyUi;
        
        //if player is dead... then
        [SerializeField] private GameObject dropKey;

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
            _photonView = GetComponent<PhotonView>();
            cam1 = GameObject.Find("CM vcam1").GetComponent<CinemachineVirtualCamera>();
            cam2 = GameObject.Find("CM vcam2").GetComponent<CinemachineVirtualCamera>();
        }

        private void Update()
        {
            if (_photonView.IsMine)
            {
                
                //collecting data
                keyUi = GameObject.FindWithTag("KeyUi").GetComponent<TextMeshProUGUI>();
                keyUi.text = _getKey.ToString();
                isCollected = _getKey>=4;
                //Movement
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
                
                
                //CinemachineVirtualCamera Lookat
                var o = gameObject;
                cam1.LookAt = o.transform;
                cam1.Follow = o.transform;
                cam2.LookAt = o.transform;
                cam2.Follow = o.transform;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (_isDead)
            {
                return;
            }
            if (other.transform.CompareTag("Key"))
            {
                _getKey++;
                Destroy(other.gameObject);
            }

            if (other.CompareTag("Bullet") || other.CompareTag("Weapon"))
            {
                playerHp--;
                _animator.SetTrigger("isHurt");
            }

            switch (_photonView.IsMine)
            {
                case true when isAttacking && other.CompareTag("Player"):
                    other.GetComponent<Player>().playerHp--;
                    break;
                case false when isAttacking && other.CompareTag("Player"):
                    playerHp--;
                    break;
            }
        }
        

        #region ANIMATIONS
        

        public void DisablePlayer()
        {
            gameObject.SetActive(false);
        }

        private void EnablePlayer()
        {
            gameObject.SetActive(true);
        }

        private void Revive()
        {
            _isDead = false;
            playerHp = 3;
            gameObject.SetActive(true);
        }
        
        
        #endregion

        //死後掉落鑰匙



        public void ReceiveDamage(Collider hit)
        {
            if (hit.transform.CompareTag("Enemy") || hit.transform.CompareTag("Bullet"))
            {
                playerHp--;
                Debug.LogError("Get hit from enemy!");
            }
        }

        private bool _isDead = false;
        private int _lostKey;
        public void PlayDamage()
        {
            if (_isDead)
            {
                return;
            }
            _animator.SetTrigger("isHurt");
            if (playerHp <= 0)
            {
                
                _isDead = true;
                playerHp = 0;
                _lostKey = _getKey;
                _getKey = 0;
                _animator.SetBool("isDead",true);
                Invoke(nameof(DisablePlayer),3);
                Invoke(nameof(DropKey),4);
                Invoke(nameof(Revive),10);
                Debug.LogError("Player is dead");
            }
        }

        private void DropKey()
        {
            
            Debug.LogError("Lost");
            
            for (int i = 0; i < _lostKey; i++)
            {
                Instantiate(dropKey, transform.position, quaternion.identity);
                
            }
        }
    }
}
