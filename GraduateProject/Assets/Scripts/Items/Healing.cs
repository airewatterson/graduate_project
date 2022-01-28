using Inventory;
using Items.Func;
using UnityEngine;

namespace Items
{
    public class Healing : ItemInfo
    {
        public void Heal()
        {
            if (GetComponentInParent<Slot>().transform.parent.CompareTag("Player"))
            {
                player1Hp += itemEffectHp;
                Debug.Log("P1 HP = " + player1Hp);
                GameManager.Instance.player1Hp = player1Hp;
                Destroy(gameObject);
            }
            else if (GetComponentInParent<Slot>().transform.parent.CompareTag("Player2"))
            {
                player2Hp += itemEffectHp;
                Debug.Log("P2 HP = " + player2Hp);
                GameManager.Instance.player2Hp = player2Hp;
                Destroy(gameObject);
            }
        }
    }
}