using System;
using General;
using UnityEngine;

namespace Item
{
    public class Spawn : SingletonMonoBehavior<Spawn>
    {
        public GameObject item;
        private Transform _player1;

        private void Start()
        {
            _player1 = GameObject.FindGameObjectWithTag("Player").transform;
        }

        public void SpawnDroppedItem()
        {
            var position = _player1.position;
            var pos = new Vector3(position.x, position.y,position.z + 1);
            Instantiate(item, pos,Quaternion.identity);
        }
    }
}
