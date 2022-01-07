using System;
using Input;
using UnityEngine;
using General;

namespace TRAP
{
    
    //看的懂。
    public class Trap : SingletonMonoBehavior<Trap>
    {
        private Player1 _player1;
        private Player2 _player2;
        private void Start()
        {
            _player1 = GameObject.FindWithTag("Player").GetComponent<Player1>();
            _player2 = GameObject.FindWithTag("Player2").GetComponent<Player2>();
        }


        private void OnTriggerEnter(Collider col)
        {
            if (col.gameObject.CompareTag("Player"))
            {
                _player1.movement-= 3f;
                print(_player1.movement);
            }
            else if (col.gameObject.CompareTag("Player2"))
            {
                _player2.movement-= 3f;
                print(_player2.movement);
            }
       
        }
        private void OnTriggerExit(Collider col)
        {
            if (col.gameObject.CompareTag("Player"))
            {
                _player1.movement += 3f;
                print(_player1.movement);
            }
            else if (col.gameObject.CompareTag("Player2"))
            {
                _player2.movement += 3f;
                print(_player2.movement);
            }
        }






    }
}
