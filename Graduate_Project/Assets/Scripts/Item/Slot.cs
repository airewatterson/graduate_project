using System;
using General;
using UnityEngine;

namespace Item
{
    public class Slot : SingletonMonoBehavior<Slot>
    {
        private Inventory _inventory;
        public int i;

        private void Start()
        {
            _inventory = GameObject.FindGameObjectWithTag("Player").GetComponent<Inventory>();
        }

        private void Update()
        {
            if (transform.childCount <= 0)
            {
                _inventory.isFull[i] = false;
            }
        }

        public void DropItem()
        {
            foreach (Transform child in transform)
            {
                child.GetComponent<Spawn>().SpawnDroppedItem();
                GameObject.Destroy(child.gameObject);
            }
        }
    }
}
