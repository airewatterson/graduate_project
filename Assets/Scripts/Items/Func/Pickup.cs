using General;
using UnityEngine;

namespace Items.Func
{
    public class Pickup : SingletonMonoBehavior<Pickup>
    {

        [SerializeField] private GameObject player1;
        private Inventory.Inventory _inventoryP1;

        [SerializeField] internal GameObject itemButton;

        private void Start()
        {
            player1 = GameManager.Instance.player1;
            _inventoryP1 = player1.GetComponent<Inventory.Inventory>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject == player1)
            {
                P1();
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
    }
}
