using System.Collections.Generic;
using UnityEngine;

public class Inventory
{
    private List<ItemController> itemList;

    public Inventory()
    {
        itemList = new List<ItemController>();
        
        //加入新物品
        AddItem(new ItemController { itemDefine = ItemController.ItemDefine.Landmine , amount = 1});
        
        Debug.Log(itemList.Count);
    }

    //加入新物品
    public void AddItem(ItemController item)
    {
        itemList.Add(item);
    }
}
