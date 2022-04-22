using Cinemachine;
using DamageSys;
using General;
using NPC;
using Photon.Pun;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Player.Input
{
    public class Player : SingletonMonoBehavior<Player>, IDamageable
    {
        [Header("玩家資訊")] 
        public float playerHp = 3;
        public float playerMaxHp = 3;
        public Image healthBar;
        [SerializeField] private float attackTimer;
        
        //attack
        [SerializeField] public Transform attackPoint;
        [SerializeField] public float attackRange;
        public LayerMask enemyLayer;
        
        
        private CharacterController _controller;
        private Vector3 _playerVelocity;

        [FormerlySerializedAs("_playerSpeed")] [SerializeField]
        private float playerSpeed = 2.0f;

        public bool isAttacking;

        private PlayerInputActions _playerInput;

        public Animator animator;

        //for Photon
        [Header("Photon設定")]
        private PhotonView _photonView;
        [SerializeField] private CinemachineVirtualCamera cam1;
        [SerializeField] private CinemachineVirtualCamera cam2;
        private CameraShake _camShake1;
        private CameraShake _camShake2;
        
        
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
            playerHp = playerMaxHp;
            _controller = GetComponent<CharacterController>();
            _photonView = GetComponent<PhotonView>();
            cam1 = GameObject.Find("CM vcam1").GetComponent<CinemachineVirtualCamera>();
            cam2 = GameObject.Find("CM vcam2").GetComponent<CinemachineVirtualCamera>();
            animator = GetComponentInChildren<Animator>();
            
            _camShake1 = GameObject.FindWithTag("cam1").GetComponent<CameraShake>();
            _camShake2 = GameObject.FindWithTag("cam2").GetComponent<CameraShake>();
        }

        private void Update()
        {
            if (_photonView.IsMine)
            {
                
                AttackTimer();
                //HP
                healthBar = GameObject.FindWithTag("HealthBar").GetComponent<Image>();
                healthBar.fillAmount = playerHp / playerMaxHp;
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
                    animator.SetBool("isRunning",true);
                }
                else
                {
                    animator.SetBool("isRunning",false);
                }

                //Attacking the other player
                if (_playerInput.Player1.Use.triggered && !isAttacking)
                {
                    Debug.LogError("attack");
                    isAttacking = true;
                    animator.SetBool("isPunching",true);
                }


                //CinemachineVirtualCamera Lookat
                var o = gameObject;
                cam1.LookAt = o.transform;
                cam1.Follow = o.transform;
                cam2.LookAt = o.transform;
                cam2.Follow = o.transform;



                PlayDamage();
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (_isDead)
            {
                return;
            }
            if (other.transform.CompareTag("Key") && _getKey < 4)
            {
                _getKey++;
                Destroy(other.gameObject);
            }
            else if (other.transform.CompareTag("Key") && _getKey >= 4)
            {
                _getKey = 4;
            }



            /*switch (_photonView.IsMine)
            {
                case true when isAttacking && other.CompareTag("Player"):
                    other.GetComponent<Player>().playerHp--;
                    break;
                case false when isAttacking && other.CompareTag("Player"):
                    playerHp--;
                    break;
            }
 
            if (other.transform.CompareTag("Enemy") )
            {
                other.GetComponent<Enemy>().enemyHp--;
                Debug.LogError("Hit Enemy");
            }*/
        }

        #region Attack Section

        public void PlayerAttack()
        {
            //_animator.SetBool("isPunching", true);
            var hit = Physics.OverlapSphere(attackPoint.position, attackRange, enemyLayer);
        
            foreach (var enemy in hit)
            {
                Debug.Log("hit" + enemy.name);
                var police = enemy.GetComponent<Enemy>();
                var player1 = enemy.gameObject.GetComponent<Player>();
                if (police != null)
                {
                    //_camShake1.ShakeCamera(5, 0.1f);
                    Debug.LogError("Detect enemy");
                    police.TakeDamage(1);
                }
                if (player1 != null)
                {
                   //_camShake1.ShakeCamera(5, 0.1f);
                    Debug.LogError("Detect player");
                    player1.playerHp--;
                }
        
            }
        }
        private void OnDrawGizmosSelected()// 確認範圍
        {
            if (attackPoint == null)
            {
                return;
            }
            Gizmos.DrawWireSphere(attackPoint.position, attackRange);
        }

        #endregion
        


        #region ANIMATIONS
        

        private void DisablePlayer()
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
            playerHp = playerMaxHp;
            gameObject.SetActive(true);
        }

        public void ReverseHurt()
        {
            animator.SetBool("isHurt",false);
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

        private void PlayDamage()
        {
            
            
            if (_isDead)
            {
                return;
            }
            
            if (playerHp <= 0)
            {
                _isDead = true;
                playerHp = 0;
                _lostKey = _getKey;
                _getKey = 0;
                animator.SetBool("isDead",true);
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

        private void AttackTimer()
        {
            if (attackTimer <= 0)
            {
                attackTimer -= Time.deltaTime;
            }
        }
    }
}
