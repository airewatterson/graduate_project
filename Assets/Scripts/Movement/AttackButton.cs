using System;
using System.Net.NetworkInformation;
using Photon.Pun;
using UnityEngine;

namespace Movement
{
    public class AttackButton : MonoBehaviourPun
    {
        private Player.Input.Player _player;
        
        public void OnAttack()
        {
            if (photonView.IsMine)
            {
                //GameObject.FindWithTag();
            }
            
        }
    }
}
