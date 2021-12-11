using System;
using UnityEngine;

namespace General.Graduate_Project
{
    public class Rules : SingletonMonoBehavior<Rules>
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                GameManager.Instance.player1CollectItem++;
            }
            else if (other.CompareTag("Player2"))
            {
                GameManager.Instance.player2CollectItem++;
            }

            Destroy(gameObject);
        }
        


    }
}
