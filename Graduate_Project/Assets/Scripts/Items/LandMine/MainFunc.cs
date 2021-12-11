using Inventory;
using Items.Func;
using UnityEngine;

namespace Items.LandMine
{
    public class MainFunc : LandMine
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other.transform.CompareTag("Player"))
            {
                Player1Hp -= itemEffectHp;
                Debug.Log("P1 HP = " + Player1Hp);
                GameManager.Instance.player1Hp = Player1Hp;
            }
            else if (other.transform.CompareTag("Player2"))
            {
                Player2Hp -= itemEffectHp;
                Debug.Log("P2 HP = " + Player2Hp);
                GameManager.Instance.player2Hp = Player2Hp;
            }

            Destroy(gameObject);
        }
    }
}