using System;
using Input;
using UnityEngine;
using General;
using UnityEngine.Serialization;

namespace TRAP
{
    
    //看的懂。
    public class Trap : SingletonMonoBehavior<Trap>
    {
        [FormerlySerializedAs("_player1")] [SerializeField]private Player1 player1;
        [FormerlySerializedAs("_player2")] [SerializeField]private Player2 player2;


        private void OnTriggerEnter(Collider col)
        {
            if (col.gameObject.CompareTag("Player"))
            {
                FindObjectOfType<AudioManager>().Play("Slip");
                player1.movement-= 3f;
                print(player1.movement);
            }
            else if (col.gameObject.CompareTag("Player2"))
            {
                FindObjectOfType<AudioManager>().Play("Slip");
                player2.movement-= 3f;
                print(player2.movement);
            }
       
        }
        private void OnTriggerExit(Collider col)
        {
            if (col.gameObject.CompareTag("Player"))
            {
                player1.movement += 3f;
                print(player1.movement);
            }
            else if (col.gameObject.CompareTag("Player2"))
            {
                player2.movement += 3f;
                print(player2.movement);
            }
        }






    }
}
