using Inventory;
using UnityEngine;

namespace Items.Func
{
    public class SpawnDefiner : Spawn
    {
        private Transform _playerDefining;

        private void Start()
        {
            GameObject.Find("GameManager").GetComponent<GameManager>();
        }

        private void Update()
        {
            if (GetComponentInParent<Slot>().transform.parent.CompareTag("Player"))
            {
                _playerDefining = player1.transform;
            }
        }
        
        

        public void SpawnObject()
        {
            var position = _playerDefining.position;
            var pos = new Vector3(position.x, position.y,position.z + 3);
            Instantiate(item, pos,Quaternion.Euler(90,0,0));
        }
    }
}
