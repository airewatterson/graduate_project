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

    [Header("P2相關參數")]
    public GameObject player2;
    //public GameObject player2HpOnUi;
    public int player2Hp;
    [SerializeField] internal TextMeshProUGUI hpTextP2;
        
    private void Update()
    {
        hpTextP1.text = player1Hp.ToString();
        hpTextP2.text = player2Hp.ToString();
            
    }

}