using System;
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
    [SerializeField] private TextMeshProUGUI player1CollectText;
    
    public int player1CollectItem;
    [SerializeField] internal bool isPlayer1Collected;
    [SerializeField] internal TextMeshProUGUI player1StatusText;
    public GameObject player1Status;

    [Header("P2相關參數")]
    public GameObject player2;
    //public GameObject player2HpOnUi;
    public int player2Hp;
    [SerializeField] internal TextMeshProUGUI hpTextP2;
    [SerializeField] private TextMeshProUGUI player2CollectText;
    
    public int player2CollectItem;
    [SerializeField] internal bool isPlayer2Collected;
    [SerializeField] internal TextMeshProUGUI player2StatusText;
    public GameObject player2Status;

    private void Start()
    {
        player1CollectItem = 0;
        player2CollectItem = 0;
    }

    private void Update()
    {
        hpTextP1.text = player1Hp.ToString();
        hpTextP2.text = player2Hp.ToString();
        player1CollectText.text = player1CollectItem.ToString();
        player2CollectText.text = player2CollectItem.ToString();
        ExitRole();

        if (player1Hp<0)
        {
            player1Hp = 0;
        }
        else if (player2Hp < 0)
        {
            player2Hp = 0;
        }
        
        DeadRule();
        
    }

    private void ExitRole()
    {
        //通關條件Flip-Flop開關
        if (player1CollectItem >= 1)
        {
            isPlayer1Collected = true;
        }
        else if (player2CollectItem >= 1)
        {
            isPlayer2Collected = true; 
        }
        else
        {
            isPlayer1Collected = false;
            isPlayer2Collected = false;
        }
    }

    private void DeadRule()
    {
        if (player1Hp <= 0)
        {
            player1.SetActive(false);
            Invoke(nameof(RespawnP1),10);
        }

        if (player2Hp <= 0)
        {
            player2.SetActive(false);
            Invoke(nameof(RespawnP2),10);
        }
    }


    private void RespawnP1()
    {
        player1Hp = 3;
        player1.SetActive(true);
    }
    private void RespawnP2()
    {
        player2Hp = 3;
        player2.SetActive(true);
    }

}