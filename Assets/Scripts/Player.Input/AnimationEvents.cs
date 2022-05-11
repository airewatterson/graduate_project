using General;
using NPC;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Player.Input
{
    public class AnimationEvents : SingletonMonoBehavior<AnimationEvents>
    {
        [Header("指定Player")]
        [SerializeField] private Player player;
        private Animator _animator;

        [Header("遊戲開始UI")] [SerializeField]
        private Animator startUI;
        [SerializeField] private GameObject mainUI;

        [Header("多人遊戲面板")]
        [SerializeField] private GameObject multiPlayerPanel;

        private void Start()
        {
            _animator = GetComponent<Animator>();
            player = GetComponentInParent<Player>();
        }

        #region Animation

        

        #endregion
        public void PlayerAttack()
        {
            var hit = Physics.OverlapSphere(player.attackPoint.position, player.attackRange, player.enemyLayer);
        
            foreach (var enemy in hit)
            {
                Debug.Log("hit" + enemy.name);
                var police = enemy.GetComponent<Enemy>();
                var player1 = enemy.gameObject.GetComponent<Player>();
                if (police != null)
                {
                    Debug.LogError("Detect enemy");
                    police.TakeDamage(1);
                }
                if (player1 != null)
                {
                    Debug.LogError("Detect player");
                    player1.playerHp--;
                }
            }

            player.isAttacking = false;
            player.animator.SetBool("isPunching",false);
        }

        public void Damage(Collider other)
        {
            if (other.gameObject.CompareTag("Enemy"))
            {
                Debug.Log("Hurt");
            }
        }


        #region Start UI

        public void GameStart()
        {
            startUI.SetTrigger("isStart");
        }

        public void MainUiStart()
        {
            mainUI.SetActive(true);
        }

        #endregion

        #region MultiPlayer Panel
        public void EnterMultiPlayerPanel()
        {
            startUI.SetBool("isMultiClicked",true);
        }
        public void ExitMultiPlayerPanel()
        {   
            startUI.SetBool("isMultiClicked",false);
        }

        #endregion

        #region EndPanel

        public void Restart()
        {
            SceneManager.LoadScene(1);
        }

        public void RestartConnect()
        {
            SceneManager.LoadScene(2);
        }

        #endregion
    }
}
