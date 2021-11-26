using System;
using UnityEngine;

namespace Item
{
    [Serializable]
    public class ItemController
    {
        /// <summary>
        /// Tutorial from https://youtu.be/2WnAOV7nHW0
        /// </summary>

        //指定有哪些物件可供使用
        public enum ItemDefine
        {
            Bomb,
            Landmine,
            Healing,
        }

        public ItemDefine itemDefine;
        public int amount;

        public Sprite GetSprite()
        {
            switch (itemDefine)
            {
                default:
                    
                case ItemDefine.Bomb:
                    return ItemAssets.Instance.bombSprite;
                case ItemDefine.Landmine:
                    return ItemAssets.Instance.landMineSprite;
                case ItemDefine.Healing:
                    return ItemAssets.Instance.healingSprite;
            }
        }

        public bool IsStackable()
        {
            switch (itemDefine)
            {
                default:
                    
                case ItemDefine.Bomb: 
                    return false;
                case ItemDefine.Healing:
                    return true;
                case ItemDefine.Landmine:
                    return false;
            }
        }
        
        
    }
}
