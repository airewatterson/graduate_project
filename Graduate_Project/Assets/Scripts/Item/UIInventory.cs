using General;
using UnityEngine;

namespace Item
{
    public class UIInventory : SingletonMonoBehavior<UIInventory>
    {
        private Inventory _inventory;
        private Transform _itemSlotContainer;
        private Transform _itemSlotTemplate;
        

        public override void Awake()
        {
            _itemSlotContainer = transform.Find("itemSlotContainer");
            _itemSlotTemplate = _itemSlotContainer.Find("itemSlotTemplate");
        }

        public void SetInventory(Inventory inventory)
        {
            _inventory = inventory;
        }

        private void RefreshInventoryItems()
        {
            
        }
    }
}
