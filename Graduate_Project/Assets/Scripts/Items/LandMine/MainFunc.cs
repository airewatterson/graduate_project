using Inventory;
using Items.Func;
using UnityEngine;

namespace Items.LandMine
{
    public class MainFunc : LandMine
    {
        private void OnTriggerEnter(Collider other)
        {
            player1Hp = GameManager.Instance.player1Hp; 
            player2Hp = GameManager.Instance.player2Hp;
            if (other.transform.CompareTag("Player"))
            {
                //player1Hp = GameManager.Instance.player1Hp; 
                player1Hp --;
                Debug.Log("P1 HP = " + player1Hp);
                GameManager.Instance.player1Hp = player1Hp;
            }
            else if (other.transform.CompareTag("Player2"))
            {
                //player2Hp = GameManager.Instance.player2Hp;
                player2Hp --;
                Debug.Log("P2 HP = " + player2Hp);
                GameManager.Instance.player2Hp = player2Hp;
            }

            Destroy(gameObject);
        }
    }
}