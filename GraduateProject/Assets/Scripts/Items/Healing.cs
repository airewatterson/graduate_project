using System;
using Input;
using Inventory;
using Items.Func;
using UnityEngine;

namespace Items
{
    public class Healing : ItemInfo
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponent<Player1>().CompareTag("Player") && Player1Hp<5)
            {
                Player1Hp += itemEffectHp;
                Debug.Log("P1 HP = " + Player1Hp);
                GameManager.Instance.player1Hp = Player1Hp;
                Destroy(gameObject);
            }
            else if (other.GetComponent<Player2>().CompareTag("Player2") && Player2Hp<5)
            {
                Player2Hp += itemEffectHp;
                Debug.Log("P2 HP = " + Player2Hp);
                GameManager.Instance.player2Hp = Player2Hp;
                Destroy(gameObject);
            }
            else
            {
                print(other.name+"血量已滿");
            }
        }
    }
}