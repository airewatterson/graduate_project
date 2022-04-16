using UnityEngine.SceneManagement;
using General;
using TMPro;
using UnityEngine;

public class GameManager : SingletonMonoBehavior<GameManager>
{
    //指定玩家
    [Header("P1相關參數")]
    //Player1 information
    public  GameObject player1;
    //public GameObject player1HpOnUi;
    public int player1Hp;
    [SerializeField] internal TextMeshProUGUI hpTextP1;
    //[SerializeField] private TextMeshProUGUI player1CollectText;
    
    public int player1CollectItem;
    [SerializeField] internal bool isPlayer1Collected;
    [SerializeField] internal TextMeshProUGUI player1StatusText;
    public GameObject player1Status;
    
    //Animation
    [SerializeField] private Animator player1Animator;

    

    //Animation
    private static readonly int IsDead = Animator.StringToHash("isDead");

    private void Start()
    {
        player1CollectItem = 0;
    }

    private void Update()
    {
        hpTextP1.text = player1Hp.ToString();
        ExitRole();

        if (player1Hp<0)
        {
            player1Hp = 0;
        }
        DeadRule();
        ResetScene();
        
    }

    private void ExitRole()
    {
        //通關條件Flip-Flop開關
        if (player1CollectItem >= 4)
        {
            isPlayer1Collected = true;
        }
        //else
        //{
        //    isPlayer1Collected = false;
        //    isPlayer2Collected = false;
        //}
    }

    private void DeadRule()
    {
        if (player1Hp <= 0)
        {
            player1Animator.SetBool(IsDead,true);
            Invoke(nameof(DisablePlayer1),3);
            Invoke(nameof(RespawnP1),10);
        }
    }

    #region DeadConfiguration

        private void RespawnP1()
        {
            player1Hp = 3;
            player1.SetActive(true);
        }

        private void DisablePlayer1()
        {
            player1Animator.SetBool("isDead",false);
            player1.SetActive(false);
        }

        #endregion

    private void ResetScene()
    {
        if (UnityEngine.Input.GetKey(KeyCode.G))
        {
            SceneManager.LoadScene("RedoScene");
        }
    }

}