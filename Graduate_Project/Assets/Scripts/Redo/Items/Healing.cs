using Redo.Inventory;
using UnityEngine;

namespace Redo.Items
{
    public class Healing : ItemInfo
    {
        public void Heal()
        {
            if (GetComponentInParent<Slot>().transform.parent.CompareTag("Player"))
            {
                Player1Hp += itemEffectHp;
                Debug.Log("P1 HP = " + Player1Hp);
                GameManager.Instance.player1Hp = Player1Hp;
            }
            else if (GetComponentInParent<Slot>().transform.parent.CompareTag("Player2"))
            {
                Player2Hp += itemEffectHp;
                Debug.Log("P2 HP = " + Player2Hp);
                GameManager.Instance.player2Hp = Player2Hp;
            }

            Destroy(gameObject);
        }
    }
}