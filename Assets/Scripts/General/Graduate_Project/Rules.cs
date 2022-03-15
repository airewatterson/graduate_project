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
                //GameManager.Instance.player1CollectItem++;
                gameObject.SetActive(false);
            }
            else if (other.CompareTag("Player2"))
            {
                //GameManager.Instance.player2CollectItem++;
                gameObject.SetActive(false);
            }

            if (GameManager.Instance.player1Hp == 0)
            {
                gameObject.SetActive(true);
                GameManager.Instance.player1CollectItem = 0;
            }
            else if (GameManager.Instance.player2Hp == 0)
            {
                gameObject.SetActive(true);
                GameManager.Instance.player2CollectItem = 0;
            }
        }
        


    }
}
