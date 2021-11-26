using CodeMonkey.Utils;
using General;
using Input;
using UnityEngine;
using UnityEngine.UI;

namespace Item
{
    public class UIInventory : SingletonMonoBehavior<UIInventory>
    {
        private Inventory _inventory;
        private Transform _itemSlotContainer;
        private Transform _itemSlotTemplate;
        
        private Player _player;
        

        public override void Awake()
        {
            _itemSlotContainer = transform.Find("itemSlotContainer");
            _itemSlotTemplate = _itemSlotContainer.Find("itemSlotTemplate");
        }

        public void SetPlayer(Player player)
        {
            _player = player;
        }

        public void SetInventory(Inventory inventory)
        {
            _inventory = inventory;
            
            inventory.onItemListChanged += Inventory_OnItemListChanged;
            
            RefreshInventoryItems();
        }

        private void Inventory_OnItemListChanged(object sender, System.EventArgs e)
        {
            RefreshInventoryItems();
        }
        
        
        private void RefreshInventoryItems()
        {
            foreach (Transform child in _itemSlotContainer)
            {
                if (child == _itemSlotTemplate)
                {
                    continue;
                }
                Destroy(child.gameObject);
            }
            
            int x = 0;
            int y = 0;
            float itemSlotCellSize = 60f;
            foreach (var item in _inventory.GetItemList())
            {
                var itemSlotRectTransform = Instantiate(_itemSlotTemplate,_itemSlotContainer).GetComponent<RectTransform>();
                itemSlotRectTransform.gameObject.SetActive(true);

                itemSlotRectTransform.GetComponent<Button_UI>().ClickFunc = () =>
                {
                    //Use item

                };
                itemSlotRectTransform.GetComponent<Button_UI>().MouseRightClickFunc = () =>
                {
                    //Drop item
                    _inventory.RemoveItem(item);
                    ItemWorld.DropItem(_player.GetPosition(), item);
                };
                
                    
                
                itemSlotRectTransform.anchoredPosition = new Vector2(x * itemSlotCellSize, y * itemSlotCellSize);
                var image = itemSlotRectTransform.Find("image").GetComponent<Image>();
                image.sprite = item.GetSprite();
                x++;

                if (x>4)
                {
                    x = 0;
                    y++;
                }
            }
        }
    }
}