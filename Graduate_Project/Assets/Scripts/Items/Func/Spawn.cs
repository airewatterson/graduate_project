﻿using General;
using Redo;
using UnityEngine;

namespace Items.Func
{
    public class Spawn : SingletonMonoBehavior<Spawn>
    {
        [SerializeField] internal GameObject item;
        [SerializeField] internal GameObject player1;
        [SerializeField] internal GameObject player2;

        private void Start()
        {
            player1 = GameManager.Instance.player1;
            player2 = GameManager.Instance.player2;
        }
    }
    
}