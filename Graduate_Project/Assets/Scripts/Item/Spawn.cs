using System;
using General;
using UnityEngine;

namespace Item
{
    public class Spawn : SingletonMonoBehavior<Spawn>
    {
        public GameObject item;
        private Transform _player1;
        private Transform _player2;

        private void Start()
        {
            _player1 = GameObject.FindGameObjectWithTag("Player").transform;
            
        }

        public void SpawnDroppedItem()
        {
            var position = _player1.position;
            var pos = new Vector3(position.x, position.y,position.z + 3);
            Instantiate(item, pos,Quaternion.Euler(90,0,0));
        }
    }
}
