using General;
using UnityEngine;

namespace Redo.Items
{
    public class Pickup : SingletonMonoBehavior<Pickup>
    {

        [SerializeField] private GameObject player1;
        private Inventory.Inventory _inventoryP1;

        [SerializeField] private GameObject player2;
        private Inventory.Inventory _inventoryP2;

        [SerializeField] internal GameObject itemButton;

        private void Start()
        {
            player1 = GameManager.Instance.player1;
            player2 = GameManager.Instance.player2;
            _inventoryP1 = player1.GetComponent<Inventory.Inventory>();
            _inventoryP2 = player2.GetComponent<Inventory.Inventory>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject == player1)
            {
                P1();
            }
            else if (other.gameObject == player2)
            {
                P2();
            }
        }

        private void P1()
        {
            for (var i = 0; i < _inventoryP1.slots.Length; i++)
            {
                if (_inventoryP1.isFull[i] == false)
                {
                    //Items can be added into inventory.
                    _inventoryP1.isFull[i] = true;
                    Instantiate(itemButton,_inventoryP1.slots[i].transform,false);
                    Destroy(gameObject);
                    break;
                }
                
            }
        }


        private void P2()
        {
            for (var i = 0; i < _inventoryP2.slots.Length; i++)
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
