using General;
using Items.Func;
using UnityEngine;

namespace Inventory
{
    public class Slot : SingletonMonoBehavior<Slot>
    {
        [SerializeField] private GameObject player1;
        [SerializeField] private GameObject player2;
        
        
        private global::Inventory.Inventory _inventoryP1;
        private global::Inventory.Inventory _inventoryP2;
        [SerializeField] private int i;

        private void Start()
        {
            _inventoryP1 = player1.GetComponent<global::Inventory.Inventory>();
            _inventoryP2 = player2.GetComponent<global::Inventory.Inventory>();
        }

        private void Update()
        {
            if (transform.childCount > 0) return;
            _inventoryP1.isFull[i] = false;
            _inventoryP2.isFull[i] = false;
        }

        public void DropItem()
        {
            foreach (Transform child in transform)
            {
                child.GetComponent<SpawnDefiner>().SpawnObject();
                Destroy(child.gameObject);
            }
        }
    }
}
