using General;
using Redo.Items;
using UnityEngine;

namespace Redo.Inventory
{
    public class Slot : SingletonMonoBehavior<Slot>
    {
        [SerializeField] private GameObject player1;
        [SerializeField] private GameObject player2;
        
        
        private Inventory _inventoryP1;
        private Inventory _inventoryP2;
        [SerializeField] private int i;

        private void Start()
        {
            _inventoryP1 = player1.GetComponent<Inventory>();
            _inventoryP2 = player2.GetComponent<Inventory>();
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
