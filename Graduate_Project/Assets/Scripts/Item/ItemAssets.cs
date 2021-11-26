using General;
using UnityEngine;

namespace Item
{
    public class ItemAssets : SingletonMonoBehavior<ItemAssets>
    {
        public Sprite landMineSprite;
        public Sprite bombSprite;
        public Sprite healingSprite;

        public Transform prefabItem;
    }
}
