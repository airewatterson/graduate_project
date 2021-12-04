using System;
using General;
using UnityEngine;

namespace Item
{
    public class PickUp : SingletonMonoBehavior<PickUp>
    {
        
        private Inventory _inventory;
        private Inventory _inventoryP2;
        public GameObject itemButton;

        private void Start()
        {
            _inventory = GameObject.FindGameObjectWithTag("Player").GetComponent<Inventory>();
            _inventoryP2 = GameObject.FindGameObjectWithTag("Player2").GetComponent<Inventory>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                for (int i = 0; i < _inventory.slots.Length; i++)
                {
                    if (_inventory.isFull[i] == false)
                    {
                        //Items can be added into inventory.
                        _inventory.isFull[i] = true;
                        Instantiate(itemButton,_inventory.slots[i].transform,false);
                        Destroy(gameObject);
                        break;
                    }
                }
            }
            else if (other.CompareTag("Player2"))
            {
                for (int i = 0; i < _inventoryP2.slots.Length; i++)
                {
                    if (_inventoryP2.isFull[i] == false)
                    {
                        //Items can be added into inventory.
                        _inventoryP2.isFull[i] = true;
                        Instantiate(itemButton,_inventoryP2.slots[i].transform,false);
                        Destroy(gameObject);
                        break;
                    }
                }
            }
            
        }
    }
}
