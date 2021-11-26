using System;
using System.Collections.Generic;
using UnityEngine;

namespace Item
{
    public class Inventory
    {
        public event EventHandler onItemListChanged;
        private List<ItemController> _itemList;

        public Inventory()
        {
            _itemList = new List<ItemController>();
        
            //加入新物品
            AddItem(new ItemController { itemDefine = ItemController.ItemDefine.Landmine , amount = 1});
            AddItem(new ItemController { itemDefine = ItemController.ItemDefine.Bomb , amount = 1});
            AddItem(new ItemController { itemDefine = ItemController.ItemDefine.Healing , amount = 1});
        
            Debug.Log(_itemList.Count);
        }

        //加入新物品
        public void AddItem(ItemController item)
        {
            if (item.IsStackable())
            {
                bool itemAlreadyInInventory = false;
                foreach (var inventoryItem in _itemList)
                {
                    if (inventoryItem.itemDefine == item.itemDefine)
                    {
                        inventoryItem.amount += item.amount;
                        itemAlreadyInInventory = true;
                    }
                }

                if (!itemAlreadyInInventory)
                {
                    _itemList.Add(item);
                }
            }
            else
            {
                _itemList.Add(item);
            }
            
            onItemListChanged?.Invoke(this, EventArgs.Empty);
        }

        public void RemoveItem(ItemController item)
        {
            if (item.IsStackable())
            {
                ItemController itemInInventory = null;
                foreach (var inventoryItem in _itemList)
                {
                    if (inventoryItem.itemDefine == item.itemDefine)
                    {
                        inventoryItem.amount -= item.amount;
                        itemInInventory = inventoryItem;
                    }
                }

                if (itemInInventory != null && itemInInventory.amount <= 0)
                {
                    _itemList.Remove(itemInInventory);
                }
            }
            else
            {
                _itemList.Remove(item);
            }
            
            onItemListChanged?.Invoke(this, EventArgs.Empty);
        }

        public List<ItemController> GetItemList()
        {
            return _itemList;
        }
    }
}
