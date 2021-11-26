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
            _itemList.Add(item);
            onItemListChanged?.Invoke(this, EventArgs.Empty);
        }

        public void RemoveItem(ItemController item)
        {
            _itemList.Remove(item);
            onItemListChanged?.Invoke(this, EventArgs.Empty);
        }

        public List<ItemController> GetItemList()
        {
            return _itemList;
        }
    }
}
