using General;
using UnityEngine;

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

        public void AnimationRevert()
        {
            player.isAttacking = false;
            _animator.SetBool("isPunching",false);
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
    }
}
